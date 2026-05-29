using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 存档管理器 — 基于 JSON 文件 + XOR 加密的持久化系统。
///
/// 特性：
///   - 文件保存在 Application.persistentDataPath（跨平台可移植）
///   - 支持 3 个独立存档槽
///   - XOR 加密防止 casual 篡改
///   - 自动从旧版 PlayerPrefs 格式迁移（向下兼容）
///   - 支持删除存档
///
/// 面试可聊：为什么选 JSON 而非 BinaryFormatter/Protobuf？为什么文件而非 PlayerPrefs？
/// </summary>
public class SaveManager : Singleton<SaveManager>
{
    private const string SaveFilePrefix = "OPJ_Save_";
    private const string SlotIndexFile = "OPJ_SaveSlots.json";
    private const string OldSaveKey = "level"; // 旧版 PlayerPrefs 场景名 key

    // XOR 加密密钥
    private readonly byte[] _key = Encoding.UTF8.GetBytes("OPJ_SaveKey_2024");

    [HideInInspector] public string sceneName = "level";

    /// <summary> 存档根目录 </summary>
    public static string SavePath => Application.persistentDataPath;

    /// <summary>
    /// 获取上次存档场景名（带文件回退，即使未 Load 也能读取）。
    /// </summary>
    public string SceneName
    {
        get
        {
            if (_currentSave != null)
                return _currentSave.sceneName;

            // 回退：直接从文件读取槽 0
            var save = LoadFromFile(0);
            return save?.sceneName ?? "";
        }
    }

    // 缓存
    private SaveSlotManager _slotManager;
    private SaveData _currentSave;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        LoadSlotIndex();
    }

    // ============================================================
    //  公共 API
    // ============================================================

    /// <summary>
    /// 保存玩家数据到指定存档槽。
    /// </summary>
    public void SavePlayerData(int slotIndex = 0, string label = "")
    {
        var stats = GameManager.Instance?.characterStats;
        if (stats == null || stats.characterData == null)
        {
            Debug.LogError("[SaveManager] 无法保存：characterStats 为空");
            return;
        }

        var cd = stats.characterData;
        var save = new SaveData
        {
            maxHealth = cd.maxHealth,
            currentHealth = cd.currentHealth,
            maxPower = cd.maxPower,
            currentPower = cd.currentPower,
            minDamage = cd.minDamage,
            maxDamage = cd.maxDamage,
            criticalMultiplier = cd.criticalMultiplier,
            criticalChance = cd.criticalChance,
            currentLevel = cd.currentLevel,
            maxLevel = cd.maxLevel,
            baseExp = cd.baseExp,
            currentExp = cd.currentExp,
            sceneName = SceneManager.GetActiveScene().name,
            characterDataName = cd.name,
            saveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            slotIndex = slotIndex,
            slotLabel = string.IsNullOrEmpty(label) ? "存档槽 " + (slotIndex + 1) : label
        };

        SaveToFile(save, slotIndex);
        UpdateSlotInfo(save);
        _currentSave = save;

        Debug.Log($"[SaveManager] 存档成功 → 槽{slotIndex + 1}: {save.sceneName} (Lv.{save.currentLevel})");
    }

    /// <summary>
    /// 从指定存档槽加载玩家数据。
    /// </summary>
    public void LoadPlayerData(int slotIndex = 0)
    {
        var save = LoadFromFile(slotIndex);
        if (save == null)
        {
            // 尝试从旧 PlayerPrefs 迁移
            save = TryMigrateFromPlayerPrefs(slotIndex);
        }

        if (save == null)
        {
            Debug.LogWarning($"[SaveManager] 槽 {slotIndex + 1} 无存档");
            return;
        }

        var stats = GameManager.Instance?.characterStats;
        if (stats == null || stats.characterData == null)
        {
            Debug.LogError("[SaveManager] 无法加载：characterStats 为空");
            return;
        }

        // 将存档数据写入 ScriptableObject
        var cd = stats.characterData;
        cd.maxHealth = save.maxHealth;
        cd.currentHealth = save.currentHealth;
        cd.maxPower = save.maxPower;
        cd.currentPower = save.currentPower;
        cd.minDamage = save.minDamage;
        cd.maxDamage = save.maxDamage;
        cd.criticalMultiplier = save.criticalMultiplier;
        cd.criticalChance = save.criticalChance;
        cd.currentLevel = save.currentLevel;
        cd.maxLevel = save.maxLevel;
        cd.baseExp = save.baseExp;
        cd.currentExp = save.currentExp;

        _currentSave = save;

        Debug.Log($"[SaveManager] 读档成功 → 槽{slotIndex + 1}: {save.sceneName} (Lv.{save.currentLevel})");
    }

    /// <summary>
    /// 删除指定存档槽。
    /// </summary>
    public void DeleteSave(int slotIndex)
    {
        string path = GetFilePath(slotIndex);
        if (File.Exists(path))
            File.Delete(path);

        if (_slotManager != null && slotIndex < _slotManager.slots.Count)
        {
            _slotManager.slots[slotIndex] = new SaveSlotInfo { index = slotIndex, isEmpty = true };
            SaveSlotIndex();
        }

        Debug.Log($"[SaveManager] 已删除存档 → 槽{slotIndex + 1}");
    }

    /// <summary>
    /// 获取当前存档（用于 SceneController 读取场景名）。
    /// </summary>
    public SaveData GetCurrentSave() => _currentSave;

    /// <summary>
    /// 获取存档槽列表（用于 UI 展示）。
    /// </summary>
    public SaveSlotManager GetSlotManager()
    {
        if (_slotManager == null)
            LoadSlotIndex();
        return _slotManager;
    }

    // ============================================================
    //  文件 I/O（核心）
    // ============================================================

    private void SaveToFile(SaveData data, int slotIndex)
    {
        string json = JsonUtility.ToJson(data, true);
        string encrypted = Encrypt(json);
        string path = GetFilePath(slotIndex);
        File.WriteAllText(path, encrypted);
    }

    private SaveData LoadFromFile(int slotIndex)
    {
        string path = GetFilePath(slotIndex);
        if (!File.Exists(path)) return null;

        try
        {
            string encrypted = File.ReadAllText(path);
            string json = Decrypt(encrypted);
            if (string.IsNullOrEmpty(json)) return null;
            return JsonUtility.FromJson<SaveData>(json);
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] 读取存档失败 → 槽{slotIndex + 1}: {e.Message}");
            return null;
        }
    }

    private static string GetFilePath(int slotIndex)
    {
        return Path.Combine(SavePath, $"{SaveFilePrefix}{slotIndex}.dat");
    }

    // ============================================================
    //  存档槽索引管理
    // ============================================================

    private void LoadSlotIndex()
    {
        string path = Path.Combine(SavePath, SlotIndexFile);
        if (File.Exists(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                _slotManager = JsonUtility.FromJson<SaveSlotManager>(json);
            }
            catch
            {
                _slotManager = new SaveSlotManager();
            }
        }
        else
        {
            _slotManager = new SaveSlotManager();
        }

        // 同步文件系统中的实际状态
        for (int i = 0; i < _slotManager.slots.Count; i++)
        {
            bool fileExists = File.Exists(GetFilePath(i));
            _slotManager.slots[i].isEmpty = !fileExists;
        }
    }

    private void SaveSlotIndex()
    {
        string path = Path.Combine(SavePath, SlotIndexFile);
        string json = JsonUtility.ToJson(_slotManager, true);
        File.WriteAllText(path, json);
    }

    private void UpdateSlotInfo(SaveData data)
    {
        if (_slotManager == null) LoadSlotIndex();
        if (data.slotIndex >= _slotManager.slots.Count) return;

        _slotManager.slots[data.slotIndex] = new SaveSlotInfo
        {
            index = data.slotIndex,
            isEmpty = false,
            label = data.slotLabel,
            saveTime = data.saveTime,
            sceneName = data.sceneName,
            level = data.currentLevel
        };

        SaveSlotIndex();
    }

    // ============================================================
    //  旧版 PlayerPrefs 迁移（向下兼容）
    // ============================================================

    private SaveData TryMigrateFromPlayerPrefs(int targetSlot)
    {
        // 使用旧版 API 的 key（character data name）检测是否有旧存档
        var stats = GameManager.Instance?.characterStats;
        if (stats == null || stats.characterData == null) return null;

        string oldKey = stats.characterData.name;
        if (!PlayerPrefs.HasKey(oldKey)) return null;

        try
        {
            string encrypted = PlayerPrefs.GetString(oldKey);
            string json = Decrypt(encrypted);
            if (string.IsNullOrEmpty(json)) return null;

            var oldData = JsonUtility.FromJson<SaveData>(json);
            if (oldData == null) return null;

            oldData.slotIndex = targetSlot;
            oldData.slotLabel = "迁移自旧版存档";

            // 保存为新格式
            SaveToFile(oldData, targetSlot);
            UpdateSlotInfo(oldData);

            // 清除旧 PlayerPrefs（可选，保留以备用）
            // PlayerPrefs.DeleteKey(oldKey);
            // PlayerPrefs.DeleteKey(OldSaveKey);

            Debug.Log($"[SaveManager] 已从旧版 PlayerPrefs 迁移存档 → 槽{targetSlot + 1}");
            return oldData;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[SaveManager] 旧版存档迁移失败: {e.Message}");
            return null;
        }
    }

    // ============================================================
    //  XOR 加密/解密
    // ============================================================

    private string Encrypt(string input)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        byte[] encrypted = new byte[inputBytes.Length];
        for (int i = 0; i < inputBytes.Length; i++)
            encrypted[i] = (byte)(inputBytes[i] ^ _key[i % _key.Length]);
        return Convert.ToBase64String(encrypted);
    }

    private string Decrypt(string input)
    {
        try
        {
            byte[] encrypted = Convert.FromBase64String(input);
            byte[] decrypted = new byte[encrypted.Length];
            for (int i = 0; i < encrypted.Length; i++)
                decrypted[i] = (byte)(encrypted[i] ^ _key[i % _key.Length]);
            return Encoding.UTF8.GetString(decrypted);
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] 解密失败: {e.Message}");
            return null;
        }
    }
}
