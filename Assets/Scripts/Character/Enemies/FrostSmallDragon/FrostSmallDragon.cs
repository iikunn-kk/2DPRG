using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostSmallDragon : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        patrolState = new FrostSmallDragonPatrolState();
        chaseState = new FrostSmallDragonChaseState();
        attackState = new FrostSmallDragonAttackState();
    }
}
