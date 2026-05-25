using UnityEngine;

/// <summary>
/// 攻击数据 ScriptableObject
/// 定义角色的攻击力、暴击率等属性
/// </summary>
[CreateAssetMenu(fileName = "New Attack", menuName = "Character Stats/Attack Data")]
public class AttackData_SO : ScriptableObject
{
    public float minDamage;
    public float maxDamage;

    [Tooltip("暴击伤害倍率")]
    public float criticalMultiplier;

    [Tooltip("暴击率 (0~1)")]
    public float criticalChance;
}
