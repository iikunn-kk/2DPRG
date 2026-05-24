using UnityEngine;

/// <summary>
/// 敌人配置数据资产，存储每个敌人类型的差异化参数。
/// 创建方式：Project 面板右键 → Create → Enemy → Enemy Config
/// </summary>
[CreateAssetMenu(fileName = "EnemyConfig", menuName = "Enemy/Enemy Config")]
public class EnemyConfig_SO : ScriptableObject
{
    [Header("移动参数")]
    public float normalSpeed = 2f;
    public float chaseSpeed = 4f;

    [Header("感知参数")]
    public float chaseRange = 5f;       // 进入追击的距离（BOX检测已实现，此参数预留）
    public float attackRange = 2.5f;     // 进入攻击范围的距离
    public float lostTime = 3f;          // 丢失目标后等待多久切回巡逻

    [Header("行为参数")]
    public float wallFlipDelay = 0.2f;  // 碰墙后翻转延迟（防止连续检测）
    public float waitTime = 2f;          // 巡逻碰墙后 idle 等待时间
    public float hurtForce = 5f;         // 受击时击退力度
    public bool hasAttackState = true;    // 该敌人是否有攻击状态（部分敌人只有巡逻+追击）

    [Header("攻击参数（hasAttackState=true 时生效）")]
    public float attackDamage = 10f;
    public float attackRate = 2f;        // 攻击间隔（秒）
    public float attackRangeRadius = 1.5f; // 攻击触发范围半径
}
