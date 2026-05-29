using System.Collections;
using UnityEngine;

/// <summary>
/// 敌人基类。
/// 作为 EnemyFsm 的数据容器，提供字段访问和事件转发。
/// 所有敌人已迁移至 EnemyFsm，旧版 FSM 代码已删除。
///
/// 架构说明：
///   Prefab 上需要同时挂载 Enemy（数据容器）+ EnemyFsm（状态机）两个组件。
///   FrostSmallDragon / IceWolf 等子类为空壳（仅用于 Inspector 识别），
///   所有行为由通用 EnemyFsm + EnemyConfig_SO 驱动，无需每个敌人写独立脚本。
///   [RequireComponent] 确保挂载 Enemy 时自动添加 EnemyFsm。
/// </summary>
[RequireComponent(typeof(EnemyFsm))]
public class Enemy : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator anim;
    [HideInInspector] public PhysicsCheck physicsCheck;
    [HideInInspector] public EnemiesAudio enemiesAudio;

    [Header("基本参数")]
    public float normalSpeed;
    public float chaseSpeed;
    public float currentSpeed;
    public Vector3 faceDir;
    public float hurtForce;
    public Transform attacker;

    [Header("检测前方是否有玩家的变量")]
    public Vector2 centerOffset;
    public Vector2 checkSize;
    public float checkDistance;
    public LayerMask attackLayer;

    [Header("延时检测计时器")]
    public float flipDelay = 0.2f;
    public float flipTimer = 0f;

    [Header("碰墙Idle计时器")]
    public float waitTime = 2f;
    public float waitTimeCounter;
    public bool wait;

    public float lostTime;
    public float lostTimeCounter;

    [Header("状态（保留字段以兼容动画事件）")]
    public bool isHurt;
    public bool isDead;

    // ========== 新版 FSM 引用 ==========
    private EnemyFsm _fsm;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        physicsCheck = GetComponent<PhysicsCheck>();
        enemiesAudio = GetComponent<EnemiesAudio>();
        _fsm = GetComponent<EnemyFsm>(); // 所有敌人已挂载 EnemyFsm
        currentSpeed = normalSpeed;
        waitTimeCounter = waitTime;
    }

    // ============================================================
    //  事件转发（由 Character.cs 的 UnityEvent 调用）
    // ============================================================

    /// <summary>
    /// 受击事件转发 -> EnemyFsm.OnTakeDamage()
    /// </summary>
    public void OnTakeDamage(Transform attackTrans)
    {
        if (_fsm != null)
        {
            _fsm.OnTakeDamage(attackTrans);
        }
    }

    /// <summary>
    /// 死亡事件转发 -> EnemyFsm.OnDie()
    /// </summary>
    public void OnDie()
    {
        if (_fsm != null)
        {
            _fsm.OnDie();
        }
    }

    /// <summary>
    /// 由动画事件调用，子类可 override
    /// </summary>
    protected virtual void DestroyAfterAnimation()
    {
        // 由动画事件调用，子类可 override
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position + (Vector3)centerOffset +
            new Vector3(checkDistance * -transform.localScale.x, 0),
            0.2f
        );
    }
}
