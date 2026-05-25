using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// 存档管理器 - 负责玩家数据的保存和加载
/// 使用 XOR 加密防止玩家篡改存档数据
/// </summary>
public class SaveManager : Singleton<SaveManager>
{
    // 存储场景名称的键值
    // string sceneName = "level";
    [HideInInspector] public string sceneName;

    // 加密密钥（简单 XOR 加密，仅供防止 casual 篡改）
    private readonly byte[] encryptionKey = Encoding.UTF8.GetBytes("OPJ_SaveKey_2024");

    // private PlayerInputController inputControl;

    /// <summary>
    /// 获取存储的场景名称（从PlayerPrefs读取）
    /// </summary>
    public string SceneName { get { return PlayerPrefs.GetString(sceneName); } }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this); // 保持跨场景不销毁
    }


    // private void OnEnable()
    // {
    //     if (inputControl != null)
    //     {
    //         inputControl.Enable();
    //     }
    // }

    // private void OnDisable()
    // {
    //     if (inputControl != null)
    //     {
    //         inputControl.Disable();
    //     }
    // }
    void Update()
    {
        // // 按键事件处理
        // if (Input.GetKeyDown(KeyCode.Escape))
        // {
        //     SceneController.Instance.TransitionToMain();
        // }
        // if (Input.GetKeyDown(KeyCode.I)) // 保存快捷键
        // {
        //     SavePlayerData();
        //     Debug.Log("保存数据");
        // }
        // if (Input.GetKeyDown(KeyCode.O)) // 加载快捷键
        // {
        //     LoadPlayerData();
        //     Debug.Log("加载数据");
        // }
    }

    /// <summary>
    /// 保存玩家数据（调用通用保存方法）
    /// </summary>
    public void SavePlayerData()
    {
        Save(GameManager.Instance.characterStats.characterData,
            GameManager.Instance.characterStats.characterData.name);
    }

    /// <summary>
    /// 加载玩家数据（调用通用加载方法）
    /// </summary>
    public void LoadPlayerData()
    {
        Load(GameManager.Instance.characterStats.characterData,
            GameManager.Instance.characterStats.characterData.name);
    }

    /// <summary>
    /// 通用保存方法（带加密）
    /// </summary>
    /// <param name="data">要保存的数据对象</param>
    /// <param name="key">存储键值</param>
    public void Save(Object data, string key)
    {
        if (data == null)
        {
            Debug.LogError($"[SaveManager] 尝试保存空数据，key: {key}");
            return;
        }

        var jsonData = JsonUtility.ToJson(data, true); // 序列化为JSON
        string encryptedData = Encrypt(jsonData); // 加密
        PlayerPrefs.SetString(key, encryptedData); // 保存加密后的数据
        PlayerPrefs.SetString(sceneName, SceneManager.GetActiveScene().name); // 保存当前场景
        Debug.Log("当前保存的场景是：" + SceneManager.GetActiveScene().name);
        PlayerPrefs.Save(); // 立即写入磁盘
    }

    /// <summary>
    /// 通用加载方法（带解密）
    /// </summary>
    /// <param name="data">要加载到的数据对象</param>
    /// <param name="key">存储键值</param>
    public void Load(Object data, string key)
    {
        if (data == null)
        {
            Debug.LogError($"[SaveManager] 尝试加载数据到空对象，key: {key}");
            return;
        }

        if (PlayerPrefs.HasKey(key))
        {
            string encryptedData = PlayerPrefs.GetString(key);
            string jsonData = Decrypt(encryptedData); // 解密

            if (string.IsNullOrEmpty(jsonData))
            {
                Debug.LogError($"[SaveManager] 解密失败，key: {key}");
                return;
            }

            JsonUtility.FromJsonOverwrite(jsonData, data); // 覆盖现有数据
        }
        else
        {
            Debug.LogWarning($"[SaveManager] 未找到存档数据，key: {key}");
        }
    }

    /// <summary>
    /// XOR 加密字符串
    /// </summary>
    private string Encrypt(string input)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        byte[] encryptedBytes = new byte[inputBytes.Length];

        for (int i = 0; i < inputBytes.Length; i++)
        {
            encryptedBytes[i] = (byte)(inputBytes[i] ^ encryptionKey[i % encryptionKey.Length]);
        }

        return System.Convert.ToBase64String(encryptedBytes);
    }

    /// <summary>
    /// XOR 解密字符串
    /// </summary>
    private string Decrypt(string input)
    {
        try
        {
            byte[] encryptedBytes = System.Convert.FromBase64String(input);
            byte[] decryptedBytes = new byte[encryptedBytes.Length];

            for (int i = 0; i < encryptedBytes.Length; i++)
            {
                decryptedBytes[i] = (byte)(encryptedBytes[i] ^ encryptionKey[i % encryptionKey.Length]);
            }

            return Encoding.UTF8.GetString(decryptedBytes);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveManager] 解密失败: {e.Message}");
            return null;
        }
    }

    /// <summary>
    /// 测试用方法：增加角色最大生命值
    /// </summary>
    public void TestAddHealth()
    {
        if (GameManager.Instance != null && GameManager.Instance.characterStats != null)
        {
            GameManager.Instance.characterStats.MaxHealth += 100;
        }
        else
        {
            Debug.LogWarning("[SaveManager] GameManager.Instance 或 characterStats 为空，无法增加生命值");
        }
    }
}
