/// <summary>
/// 敌人有限状态机的状态枚举。
/// 包含完整的五个状态，Hurt 和 Dead 从此成为正式状态（不再是 bool hack）。
/// </summary>
public enum EnemyFsmState
{
    Patrol,   // 巡逻
    Chase,    // 追击
    Attack,   // 攻击
    Hurt,     // 受击（可被打断，结束后自动回到 Patrol/Chase）
    Dead      // 死亡（终态，不退出）
}
