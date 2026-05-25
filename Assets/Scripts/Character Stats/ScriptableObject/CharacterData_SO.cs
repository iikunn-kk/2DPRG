using UnityEngine;

/// <summary>
/// 角色数据 ScriptableObject
/// 管理角色所有基础属性与升级逻辑
/// </summary>
[CreateAssetMenu(fileName = "New Data", menuName = "Character Stats/Data")]
public class CharacterData_SO : ScriptableObject
{
    [Header("血量属性")]
    public int maxHealth;
    public int currentHealth;

    [Header("攻击属性")]
    public float minDamage;
    public float maxDamage;
    public float criticalMultiplier;
    public float criticalChance;

    [Header("滑铲能量条")]
    public float maxPower;
    public float currentPower;

    [Header("击杀后掉落的经验值")]
    public int killPoint;

    [Header("等级属性")]
    public int currentLevel = 1;
    public int maxLevel = 10;
    public int baseExp;
    public int currentExp;

    [Tooltip("升一级属性提升的百分比 (如 0.1 = 10%)")]
    [SerializeField] private float _levelBuff = 0.1f;

    [Header("升级增量")]
    [SerializeField] private float _powerPerLevel = 2.5f;

    public float LevelMultiplier => 1 + (currentLevel - 1) * _levelBuff;

    public void UpdateExp(int point)
    {
        currentExp += point;
        if (currentExp >= baseExp)
            LevelUp();
    }

    private void LevelUp()
    {
        currentLevel = Mathf.Clamp(currentLevel + 1, 0, maxLevel);
        baseExp += (int)(baseExp * LevelMultiplier);

        maxHealth = (int)(maxHealth * LevelMultiplier);
        currentHealth = maxHealth;
        minDamage += 1;
        maxDamage += 1;
        maxPower += _powerPerLevel;
        currentPower = maxPower;
    }
}
