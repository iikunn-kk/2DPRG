using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagmaDemon : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        patrolState = new MagmaDemonPatrolState();
        chaseState = new MagmaDemonChaseState();
        attackState = new MagmaDemonAttackState();
    }
}
