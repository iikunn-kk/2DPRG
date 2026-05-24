using UnityEngine;

/// <summary>
/// 哥布林二型。
/// 已迁移至通用 EnemyFsm 系统。
/// 注意：无攻击状态，需在 EnemyConfig_SO 中设置 hasAttackState = false。
/// </summary>
public class GoblinTwo : Enemy
{
    // 所有状态行为由 EnemyFsm 驱动。
}
