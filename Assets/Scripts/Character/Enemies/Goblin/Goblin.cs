using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        patrolState = new GoblinPatrolState();
        chaseState = new GoblinChaseState();
    }
}
