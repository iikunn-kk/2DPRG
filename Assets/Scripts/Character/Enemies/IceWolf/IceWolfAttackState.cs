using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceWolfAttackState : BaseState
{
    private Attack attack;
    private float attackRateCounter;
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        Debug.Log("进入攻击状态");
        // currentEnemy.currentSpeed = 0f;
        attackRateCounter = 2f;
        currentEnemy.anim.SetTrigger("attack");
        // GameObject.FindGameObjectWithTag("Ice1").GetComponent<CrystalAudio>().PlayAttackAudio();

        currentEnemy.anim.SetBool("walk", false);
        attack = currentEnemy.GetComponent<Attack>();
    }
    public override void LogicUpdate()
    {
        //当怪物发现玩家并且触发追击状态，当距离小于2.5的时候，就进入攻击状态
        if (currentEnemy.FoundPlayer() && Mathf.Abs(currentEnemy.transform.position.x - GameObject.FindGameObjectWithTag("Player").transform.position.x) <= 4f)
        {
            if (currentEnemy.isDead == false)
            {
                //攻击频率计时器
                attackRateCounter -= Time.deltaTime;
                // Debug.Log(attackRateCounter);
                if (attackRateCounter <= 0)
                {
                    // GameObject.FindGameObjectWithTag("Ice1").GetComponent<CrystalAudio>().PlayAttackAudio();
                    Debug.Log("执行攻击动画");
                    currentEnemy.anim.SetTrigger("attack");
                    // attackRateCounter = attack.attackRate;
                }
            }
        }

        if (currentEnemy.FoundPlayer() && Mathf.Abs(currentEnemy.transform.position.x - GameObject.FindGameObjectWithTag("Player").transform.position.x) > 2.5f)
        {
            Debug.Log("切换到追击状态");
            currentEnemy.SwitchState(EnemyState.Chase);
        }

        if (!currentEnemy.FoundPlayer())
        {
            Debug.Log("切换到巡逻状态");
            currentEnemy.SwitchState(EnemyState.Patrol);
        }
    }

    public override void PhysicsUpdate()
    {

    }
    public override void OnExit()
    {
        // if (currentEnemy.physicsCheck.touchLeftWall || currentEnemy.physicsCheck.touchRightWall || currentEnemy.physicsCheck.touchLeftAirWall || currentEnemy.physicsCheck.touchRightAirWall)
        // {

        //     currentEnemy.wait = true;
        //     currentEnemy.anim.SetBool("walk", false);
        //     //currentEnemy.currentSpeed = 0;
        // }
        // if (currentEnemy.physicsCheck.touchLeftWall || currentEnemy.physicsCheck.touchRightWall || currentEnemy.physicsCheck.touchLeftAirWall || currentEnemy.physicsCheck.touchRightAirWall)
        // {
        //     //currentEnemy.isWait = false;
        //     currentEnemy.anim.SetBool("walk", true);
        //     //currentEnemy.currentSpeed = currentEnemy.normalSpeed;
        // }
        Debug.Log("退出攻击状态");
    }
}
