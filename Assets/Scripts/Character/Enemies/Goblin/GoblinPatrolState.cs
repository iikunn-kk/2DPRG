using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinPatrolState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.normalSpeed;
        Debug.Log("哥布林进入巡逻状态");
    }
    public override void LogicUpdate()
    {
        //TODO发现敌人切换到追击状态
        if (currentEnemy.FoundPlayer())
        {
            currentEnemy.SwitchState(EnemyState.Chase);
        }
        // 只有当翻转计时器为0时才进行墙检测
        if (currentEnemy.flipTimer <= 0)
        {
            if (currentEnemy.physicsCheck.touchLeftWall || currentEnemy.physicsCheck.touchRightWall || currentEnemy.physicsCheck.touchLeftAirWall || currentEnemy.physicsCheck.touchRightAirWall)
            {
                currentEnemy.wait = true;
                currentEnemy.anim.SetBool("walk", false);
                currentEnemy.rb.velocity = Vector2.zero;  // 敌人静止
                // 重置翻转计时器
                currentEnemy.flipTimer = currentEnemy.flipDelay;
            }
            else
            {
                currentEnemy.anim.SetBool("walk", true);
            }
        }
    }

    public override void PhysicsUpdate()
    {

    }
    public override void OnExit()
    {
        currentEnemy.anim.SetBool("walk", false);
        Debug.Log("哥布林退出巡逻状态");
    }
}
