using UnityEngine;

/// <summary>
/// 攻击判定组件
/// 挂载在攻击碰撞体上，负责检测攻击范围内的目标并触发伤害
/// </summary>
public class Attack : MonoBehaviour
{
    [Header("攻击配置")]
    [SerializeField] private float damage;      // 攻击伤害值（可由 CharacterStats 覆盖）
    [SerializeField] private float attackRange; // 攻击范围（备用）
    
    [Header("运行时数据（自动获取）")]
    public CharacterStats characterStats;

    void Awake()
    {
        characterStats = GetComponentInParent<CharacterStats>();
        
        if (characterStats == null)
        {
            Debug.LogWarning($"[Attack] 未找到父级 CharacterStats 组件: {gameObject.name}");
        }
    }

    /// <summary>
    /// 攻击碰撞检测（每帧调用）
    /// 当其他 Collider2D 进入攻击范围时触发伤害
    /// </summary>
    public void OnTriggerStay2D(Collider2D collision)
    {
        // 尝试获取目标的 Character 组件并造成伤害
        if (collision.TryGetComponent(out Character targetCharacter))
        {
            targetCharacter.TakeDamage(this);
        }

        // 特殊处理：木头人训练目标
        if (collision.CompareTag("WoodMan"))
        {
            if (collision.TryGetComponent(out WoodManHurt woodMan))
            {
                woodMan.HitFromPlayer();
            }
        }
    }
}
