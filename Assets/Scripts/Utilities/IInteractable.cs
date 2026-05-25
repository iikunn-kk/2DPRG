/// <summary>
/// 可交互对象接口
/// 实现此接口的对象可被玩家的确认键触发交互
/// </summary>
public interface IInteractable
{
    void TriggerAction();
}
