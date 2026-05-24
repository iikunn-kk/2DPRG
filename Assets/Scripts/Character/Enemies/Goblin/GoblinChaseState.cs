using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinChaseState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        Debug.Log("哥布林进入追击状态");
        currentEnemy.currentSpeed = currentEnemy.chaseSpeed;
        currentEnemy.anim.SetBool("run", true);
    }

    public override void LogicUpdate()
    {

        if (currentEnemy.lostTimeCounter <= 0)
        {
            currentEnemy.SwitchState(EnemyState.Patrol);
        }
        if (currentEnemy.flipTimer <= 0)
        {
            if (currentEnemy.physicsCheck.touchLeftWall || currentEnemy.physicsCheck.touchRightWall || currentEnemy.physicsCheck.touchLeftAirWall || currentEnemy.physicsCheck.touchRightAirWall)
            {
                currentEnemy.transform.localScale = new Vector3(currentEnemy.faceDir.x, currentEnemy.transform.localScale.y, currentEnemy.transform.localScale.z);
                // 敌人追击状态下直接翻转，不用等待，但也需要翻转计时器的功能延迟再检测，不然会持续检测，导致敌人在原地来回检测，不断的翻转引起bug
                // 重置翻转计时器
                currentEnemy.flipTimer = currentEnemy.flipDelay;
            }
        }
    }

    public override void PhysicsUpdate()
    {

    }

    public override void OnExit()
    {
        Debug.Log("哥布林退出追击状态");
        currentEnemy.lostTimeCounter = currentEnemy.lostTime;
        currentEnemy.anim.SetBool("run", false);
    }
}
