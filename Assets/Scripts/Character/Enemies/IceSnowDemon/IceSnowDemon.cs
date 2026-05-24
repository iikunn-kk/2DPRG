using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSnowDemon : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        patrolState = new IceSnowDemonPatrolState();
        chaseState = new IceSnowDemonChaseState();
        attackState = new IceSnowDemonAttackState();

    }
}
