using UnityEngine;

/// <summary>
/// 冰霜蘑菇。
/// 已迁移至通用 EnemyFsm 系统。
/// 注意：攻击距离较远（2.5f），需在 EnemyConfig_SO 中设置 attackRange = 2.5f。
/// </summary>
public class IceSnowMushrooms : Enemy
{
    // 所有状态行为由 EnemyFsm 驱动。
}
