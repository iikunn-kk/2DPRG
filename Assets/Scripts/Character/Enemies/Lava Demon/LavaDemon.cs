using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaDemon : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        patrolState = new LavaDemonPatrolState();
        chaseState = new LavaDemonChaseState();
        attackState = new LavaDemonAttackState();
    }
}
