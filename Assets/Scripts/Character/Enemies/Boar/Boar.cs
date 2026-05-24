using UnityEngine;

/// <summary>
/// 野猪敌人。
/// 已迁移至通用 EnemyFsm 系统，不再需要 BoarPatrolState / BoarChaseState 文件。
/// 使用方式：将 EnemyFsm 组件挂载到同一 GameObject，
/// 并在 Inspector 中拖入对应的 EnemyConfig_SO 资产。
/// </summary>
public class Boar : Enemy
{
    // 不需要 override 任何逻辑。
    // 所有状态行为由 EnemyFsm 驱动。
    // 如果野猪有特殊行为（如冲锋攻击），可以在此添加额外组件或 override EnemyFsm 的事件方法。

    protected override void DestroyAfterAnimation()
    {
        // 野猪死亡动画播放完毕后销毁（由动画事件调用）
        base.DestroyAfterAnimation();
    }
}
