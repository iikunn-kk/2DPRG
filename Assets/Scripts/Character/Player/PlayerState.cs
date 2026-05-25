/// <summary>
/// 玩家状态枚举
/// 取代 PlayerController 中的多个 bool 标志，保证状态互斥
/// </summary>
public enum PlayerState
{
    Idle,      // 静止（默认状态）
    Run,       // 移动中
    Jump,      // 跳跃
    Fall,      // 下落
    Crouch,    // 下蹲
    Attack,    // 攻击中
    Slide,     // 滑铲中
    Hurt,      // 受击
    Dead,      // 死亡（终态）
    Dialogue   // 对话中
}
