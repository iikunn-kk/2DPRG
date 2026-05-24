/// <summary>
/// 玩家状态枚举。
/// 取代 PlayerController 中的 7 个 bool 标志（isHurt/isDead/isAttack/isCrouch/isDialogue/isJumping/isSlide）。
/// 由 SwitchTo() 保证状态互斥，避免多个 bool 同时为 true 的 bug。
/// </summary>
public enum PlayerState
{
    Idle,      // 静止（默认状态）
    Run,       // 移动中
    Jump,      // 跳跃（上升）
    Fall,      // 下落
    Crouch,    // 下蹲
    Attack,    // 攻击中
    Slide,     // 滑铲中
    Hurt,      // 受击（可被 Death 打断）
    Dead,      // 死亡（终态）
    Dialogue   // 对话中（暂停移动输入）
}
