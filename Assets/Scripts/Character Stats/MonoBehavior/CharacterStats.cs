using UnityEngine;

/// <summary>
/// 角色属性组件
/// 管理角色的生命值、攻击力、暴击率、等级、经验值等属性
/// 通过 ScriptableObject (CharacterData_SO) 实现数据驱动配置
/// </summary>
public class CharacterStats : MonoBehaviour
{
    [Header("数据模板")]
    public CharacterData_SO templateData; // 原型数据（用于实例化）

    [Header("运行时数据")]
    public CharacterData_SO characterData; // 当前角色数据实例

    #region 属性访问器

    /// <summary> 最大生命值 </summary>
    public int MaxHealth
    {
        get => characterData?.maxHealth ?? 0;
        set { if (characterData != null) characterData.maxHealth = value; }
    }

    /// <summary> 当前生命值 </summary>
    public int CurrentHealth
    {
        get => characterData?.currentHealth ?? 0;
        set { if (characterData != null) characterData.currentHealth = value; }
    }

    /// <summary> 最小伤害值 </summary>
    public float MinDamage
    {
        get => characterData?.minDamage ?? 0f;
        set { if (characterData != null) characterData.minDamage = value; }
    }

    /// <summary> 最大伤害值 </summary>
    public float MaxDamage
    {
        get => characterData?.maxDamage ?? 0f;
        set { if (characterData != null) characterData.maxDamage = value; }
    }

    /// <summary> 暴击倍率 </summary>
    public float CriticalMultiplier
    {
        get => characterData?.criticalMultiplier ?? 0f;
        set { if (characterData != null) characterData.criticalMultiplier = value; }
    }

    /// <summary> 暴击几率 (0-1) </summary>
    public float CriticalChance
    {
        get => characterData?.criticalChance ?? 0f;
        set { if (characterData != null) characterData.criticalChance = value; }
    }

    /// <summary> 当前等级 </summary>
    public int CurrentLevel
    {
        get => characterData?.currentLevel ?? 1;
        set { if (characterData != null) characterData.currentLevel = value; }
    }

    /// <summary> 最大等级 </summary>
    public int MaxLevel
    {
        get => characterData?.maxLevel ?? 99;
        set { if (characterData != null) characterData.maxLevel = value; }
    }

    /// <summary> 升级所需基础经验值 </summary>
    public int BaseExp
    {
        get => characterData?.baseExp ?? 100;
        set { if (characterData != null) characterData.baseExp = value; }
    }

    /// <summary> 当前经验值 </summary>
    public int CurrentExp
    {
        get => characterData?.currentExp ?? 0;
        set { if (characterData != null) characterData.currentExp = value; }
    }

    /// <summary> 当前能量值 </summary>
    public float CurrentPower
    {
        get => characterData?.currentPower ?? 0f;
        set { if (characterData != null) characterData.currentPower = value; }
    }

    /// <summary> 最大能量值 </summary>
    public float MaxPower
    {
        get => characterData?.maxPower ?? 100f;
        set { if (characterData != null) characterData.maxPower = value; }
    }

    /// <summary> 击杀奖励经验值 </summary>
    public int KillPoint => characterData?.killPoint ?? 10;

    #endregion

    private void Awake()
    {
        InitializeCharacterData();
    }

    /// <summary>
    /// 初始化角色数据：从模板创建独立副本
    /// 避免多个角色共享同一份数据导致属性互相影响
    /// </summary>
    private void InitializeCharacterData()
    {
        if (templateData != null)
        {
            characterData = Instantiate(templateData);
            Debug.Log($"[CharacterStats] 已初始化角色数据: {gameObject.name}");
        }
        else
        {
            Debug.LogError($"[CharacterStats] 未设置模板数据 (templateData): {gameObject.name}");
        }
    }

    /// <summary>
    /// 检查角色是否存活
    /// </summary>
    public bool IsAlive => CurrentHealth > 0;
}
