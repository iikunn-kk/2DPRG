using UnityEngine;

/// <summary>
/// 霜冻小飞龙。
/// 已迁移至通用 EnemyFsm 系统。
/// 使用方式：将 EnemyFsm 组件挂载到同一 GameObject，
/// 并在 Inspector 中拖入对应的 EnemyConfig_SO 资产。
/// </summary>
public class FrostSmallDragon : Enemy
{
    // 所有状态行为由 EnemyFsm 驱动。
    // 如果霜冻小飞龙有特殊行为（如飞行），可在此添加组件或 override。
}
