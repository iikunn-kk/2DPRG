// 移除不需要的 using 指令
using UnityEngine;

/// <summary>
/// 野猪追击状态类，继承自 BaseState，用于处理野猪在追击玩家时的行为逻辑。
/// </summary>
public class BoarChaseState : BaseState
{
    /// <summary>
    /// 当野猪进入追击状态时调用此方法，进行状态初始化操作。
    /// </summary>
    /// <param name="enemy">进入此状态的敌人实例。</param>
    public override void OnEnter(Enemy enemy)
    {
        // 将传入的敌人实例赋值给当前敌人对象
        currentEnemy = enemy;
        // 打印日志，提示野猪进入追击状态
        Debug.Log("野猪进入追击状态");
        // 将敌人的当前速度设置为追击速度
        currentEnemy.currentSpeed = currentEnemy.chaseSpeed;
        // 设置动画控制器的 "run" 参数为 true，播放奔跑动画
        currentEnemy.anim.SetBool("run", true);
    }

    /// <summary>
    /// 在每帧更新时调用此方法，处理追击状态下的逻辑判断。
    /// </summary>
    public override void LogicUpdate()
    {
        // 检查敌人丢失玩家的计时是否已到，如果计时结束则切换回巡逻状态
        if (currentEnemy.lostTimeCounter <= 0)
        {
            currentEnemy.SwitchState(EnemyState.Patrol);
        }
        // 检查翻转计时器是否已归零，只有归零后才进行墙检测
        if (currentEnemy.flipTimer <= 0)
        {
            // 检查敌人是否碰到左侧墙、右侧墙、左侧空中墙或右侧空中墙
            if (currentEnemy.physicsCheck.touchLeftWall || currentEnemy.physicsCheck.touchRightWall || currentEnemy.physicsCheck.touchLeftAirWall || currentEnemy.physicsCheck.touchRightAirWall)
            {
                // 若碰到墙，翻转敌人的朝向
                currentEnemy.transform.localScale = new Vector3(currentEnemy.faceDir.x, currentEnemy.transform.localScale.y, currentEnemy.transform.localScale.z);
                // 注释说明：敌人追击状态下直接翻转，不用等待，但也需要翻转计时器的功能延迟再检测，
                // 不然会持续检测，导致敌人在原地来回检测，不断的翻转引起bug
                // 重置翻转计时器
                currentEnemy.flipTimer = currentEnemy.flipDelay;
            }
        }
    }

    /// <summary>
    /// 在物理模拟更新时调用此方法，当前追击状态下无物理相关逻辑。
    /// </summary>
    public override void PhysicsUpdate()
    {
        // 目前此状态下没有需要在物理更新阶段执行的逻辑
    }

    /// <summary>
    /// 当野猪退出追击状态时调用此方法，进行状态退出清理操作。
    /// </summary>
    public override void OnExit()
    {
        // 打印日志，提示野猪退出追击状态
        Debug.Log("野猪退出追击状态");
        // 重置敌人丢失玩家的计时
        currentEnemy.lostTimeCounter = currentEnemy.lostTime;
        // 设置动画控制器的 "run" 参数为 false，停止播放奔跑动画
        currentEnemy.anim.SetBool("run", false);
    }
}
