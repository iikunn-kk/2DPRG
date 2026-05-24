using UnityEngine;

/// <summary>
/// 冰狼。
/// 已迁移至通用 EnemyFsm 系统。
/// 注意：攻击距离较远（4-5f），需在 EnemyConfig_SO 中设置 attackRange = 4f。
/// </summary>
public class IceWolf : Enemy
{
    // 所有状态行为由 EnemyFsm 驱动。
}
