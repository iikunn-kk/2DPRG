using System;
using System.Collections.Generic;

/// <summary>
/// 存档数据模型（纯 C# POCO，不依赖 Unity）
/// 支持 JSON 序列化，用于文件持久化。
/// </summary>
[Serializable]
public class SaveData
{
    // ===== 角色属性 =====
    public int maxHealth;
    public int currentHealth;
    public float maxPower;
    public float currentPower;
    public float minDamage;
    public float maxDamage;
    public float criticalMultiplier;
    public float criticalChance;
    public int currentLevel;
    public int maxLevel;
    public int baseExp;
    public int currentExp;

    // ===== 游戏进度 =====
    public string sceneName;           // 当前所在场景
    public string characterDataName;   // 角色数据名称（用于 SO 匹配）
    public string saveTime;            // 存档时间戳（人类可读）

    // ===== 多存档槽元信息 =====
    public int slotIndex;              // 存档槽编号（0-2）
    public string slotLabel;           // 存档槽标签（如 "自动存档"、"手动存档1"）

    // ===== 存档版本（用于未来兼容性） =====
    public int saveVersion = 1;
}

/// <summary>
/// 存档槽管理器（管理 3 个独立存档槽）
/// </summary>
[Serializable]
public class SaveSlotManager
{
    public List<SaveSlotInfo> slots = new List<SaveSlotInfo>();

    public SaveSlotManager()
    {
        // 初始化 3 个空槽
        for (int i = 0; i < 3; i++)
            slots.Add(new SaveSlotInfo { index = i, isEmpty = true });
    }
}

/// <summary>
/// 存档槽元信息（不包含实际存档数据，用于 UI 展示）
/// </summary>
[Serializable]
public class SaveSlotInfo
{
    public int index;
    public bool isEmpty;
    public string label;
    public string saveTime;
    public string sceneName;
    public int level;
}
