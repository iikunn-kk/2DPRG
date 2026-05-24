using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Attack", menuName = "Character Stats/Attack Data")]

public class AttackData_SO : ScriptableObject
{
    // public float damage;
    public float minDamage;
    public float maxDamage;
    //public float damage = Random(minDamage, maxDamage);
    public float criticalMultiplier;//暴击增幅百分比
    public float criticalChance;//暴击率（百分之20的暴击率）


}
