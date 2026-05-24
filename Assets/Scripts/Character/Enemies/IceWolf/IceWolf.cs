using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceWolf : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        patrolState = new IceWolfPatrolState();
        chaseState = new IceWolfChaseState();
        attackState = new IceWolfAttackState();
    }
    // public void DestroyIceWolfAfterAnimation()
    // {
    //     Destroy(gameObject);
    // }
}
