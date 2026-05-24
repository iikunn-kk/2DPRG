using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaDragon : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        patrolState = new LavaDragonPatrolState();
        chaseState = new LavaDragonChaseState();
        attackState = new LavaDragonAttackState();
    }
}
