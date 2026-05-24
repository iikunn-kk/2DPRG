using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public float damage;
    public float attackRange;
    public float attackRate;
    public CharacterStats characterStats;
    public void Awake()
    {
        characterStats = GetComponentInParent<CharacterStats>();
    }
    void Start()
    {
        //damage = characterStats.Damage;
    }
    public void OnTriggerStay2D(Collider2D collision)
    {

        // Debug.Log("受到了" + collision.name + "的攻击");
        collision.GetComponent<Character>()?.TakeDamage(this);
        if (collision.tag == "WoodMan")
        {
            collision.GetComponent<WoodManHurt>().HitFromPlayer();
        }
    }
}
