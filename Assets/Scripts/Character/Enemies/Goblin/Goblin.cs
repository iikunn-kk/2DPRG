public class Goblin : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        // 使用挂载的 EnemyFsm 组件驱动行为，不再需要手动初始化状态
    }
}
