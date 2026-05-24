using UnityEngine;

/// <summary>
/// 岩浆恶魔。
/// 已迁移至通用 EnemyFsm 系统。
/// 注意：攻击时不停止移动，
/// 需在 EnemyConfig_SO 中设置 attackRange = 4f。
/// </summary>
public class MagmaDemon : Enemy
{
    // 所有状态行为由 EnemyFsm 驱动。
}
