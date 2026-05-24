using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaScorpion : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        patrolState = new LavaScorpionPatrolState();
        chaseState = new LavaScorpionChaseState();
        attackState = new LavaScorpionAttackState();
    }
}
