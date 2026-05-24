using System.Collections;
using System.Collections.Generic;
// 移除不需要的 using 指令
using UnityEngine;

/// <summary>
/// 野猪巡逻状态类，继承自 BaseState 抽象类，用于处理野猪在巡逻状态下的行为逻辑。
/// </summary>
public class BoarPatrolState : BaseState
{
    /// <summary>
    /// 当野猪进入巡逻状态时调用此方法，进行状态初始化操作。
    /// </summary>
    /// <param name="enemy">进入此状态的敌人实例。</param>
    public override void OnEnter(Enemy enemy)
    {
        // 将传入的敌人实例赋值给当前敌人对象
        currentEnemy = enemy;
        // 将敌人的当前速度设置为正常巡逻速度
        currentEnemy.currentSpeed = currentEnemy.normalSpeed;
        // 打印日志，提示进入巡逻状态
        Debug.Log("进入巡逻状态");
    }

    /// <summary>
    /// 在每帧更新时调用此方法，处理巡逻状态下的逻辑判断。
    /// </summary>
    public override void LogicUpdate()
    {
        // 检查敌人是否发现玩家，如果发现则切换到追击状态
        if (currentEnemy.FoundPlayer())
        {
            currentEnemy.SwitchState(EnemyState.Chase);
        }

        // 只有当翻转计时器为 0 时才进行墙检测
        if (currentEnemy.flipTimer <= 0)
        {
            // 检查敌人是否碰到左侧墙、右侧墙、左侧空中墙或右侧空中墙
            if (currentEnemy.physicsCheck.touchLeftWall || currentEnemy.physicsCheck.touchRightWall || currentEnemy.physicsCheck.touchLeftAirWall || currentEnemy.physicsCheck.touchRightAirWall)
            {
                // 若碰到墙，设置敌人进入等待状态
                currentEnemy.wait = true;
                // 设置动画控制器的 "walk" 参数为 false，停止播放行走动画
                currentEnemy.anim.SetBool("walk", false);
                // 让敌人静止，将其刚体速度设置为零
                currentEnemy.rb.velocity = Vector2.zero;
                // 重置翻转计时器，避免持续检测导致异常
                currentEnemy.flipTimer = currentEnemy.flipDelay;
            }
            else
            {
                // 若未碰到墙，设置动画控制器的 "walk" 参数为 true，播放行走动画
                currentEnemy.anim.SetBool("walk", true);
            }
        }
    }

    /// <summary>
    /// 在物理模拟更新时调用此方法，当前巡逻状态下无物理相关逻辑。
    /// </summary>
    public override void PhysicsUpdate()
    {
        // 目前此状态下没有需要在物理更新阶段执行的逻辑
    }

    /// <summary>
    /// 当野猪退出巡逻状态时调用此方法，进行状态退出清理操作。
    /// </summary>
    public override void OnExit()
    {
        // 设置动画控制器的 "walk" 参数为 false，停止播放行走动画
        currentEnemy.anim.SetBool("walk", false);
        // 打印日志，提示退出巡逻状态
        Debug.Log("退出巡逻状态");
    }
}
