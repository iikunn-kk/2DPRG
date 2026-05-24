using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSnowMushrooms : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        patrolState = new IceSnowMushroomsPatrolState();
        chaseState = new IceSnowMushroomsChaseState();
        attackState = new IceSnowMushroomsAttackState();
    }
}
