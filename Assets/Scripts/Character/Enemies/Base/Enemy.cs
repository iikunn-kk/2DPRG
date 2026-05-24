using System.Collections;
using UnityEngine;

/// <summary>
/// 敌人基类。
/// 兼容模式：优先使用挂载的 EnemyFsm 组件；若未挂载则回退旧版 BaseState 逻辑。
/// 这样可以在迁移过程中逐个敌人切换，不影响未迁移的敌人。
/// </summary>
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

    // ========== 旧版状态机（迁移完成后可删除）==========
    private BaseState currentState;
    protected BaseState patrolState;
    protected BaseState chaseState;
    protected BaseState attackState;

    // ========== 新版 FSM 引用 ==========
    private EnemyFsm _fsm;

    // 标记是否使用新版 FSM
    private bool UseFsm => _fsm != null;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        physicsCheck = GetComponent<PhysicsCheck>();
        enemiesAudio = GetComponent<EnemiesAudio>();
        _fsm = GetComponent<EnemyFsm>(); // 如果没有挂载 EnemyFsm，则为 null
        currentSpeed = normalSpeed;
        waitTimeCounter = waitTime;
    }

    void OnEnable()
    {
        if (UseFsm)
        {
            // FSM 的 OnEnable 内部会切换到 Patrol，这里不需要额外操作
        }
        else
        {
            currentState = patrolState;
            if (currentState != null)
                currentState.OnEnter(this);
        }
    }

    private void Update()
    {
        faceDir = new Vector3(-transform.localScale.x, 0, 0);

        if (flipTimer > 0)
        {
            flipTimer -= Time.deltaTime;
        }

        if (!UseFsm && currentState != null)
        {
            currentState.LogicUpdate();
            TimeCounter();
        }
        // FSM 的 Update 由 EnemyFsm 自己驱动，不需要这里调用
    }

    private void FixedUpdate()
    {
        if (UseFsm)
        {
            // FSM 的 FixedUpdate 由 EnemyFsm 自己驱动
        }
        else
        {
            // 修复原 bug：将按位与 & 改为逻辑与 &&
            if (!isHurt && !isDead)
                Move();
            if (currentState != null)
                currentState.PhysicsUpdate();
        }
    }

    private void OnDisable()
    {
        if (!UseFsm && currentState != null)
        {
            currentState.OnExit();
        }
    }

    public virtual void Move()
    {
        if (!wait)
        {
            rb.velocity = new Vector2(currentSpeed * faceDir.x, rb.velocity.y);
            anim.SetBool("walk", true);
        }
    }

    public void TimeCounter()
    {
        if (wait)
        {
            waitTimeCounter -= Time.deltaTime;
            if (waitTimeCounter <= 0)
            {
                wait = false;
                waitTimeCounter = waitTime;
                transform.localScale = new Vector3(faceDir.x, transform.localScale.y, transform.localScale.z);
            }
        }
        if (!FoundPlayer() && lostTimeCounter > 0)
        {
            lostTimeCounter -= Time.deltaTime;
        }
        if (FoundPlayer())
        {
            lostTimeCounter = lostTime;
        }
    }

    public bool FoundPlayer()
    {
        return Physics2D.BoxCast(transform.position + (Vector3)centerOffset, checkSize, 0, faceDir, checkDistance, attackLayer);
    }

    // 保留旧版 SwitchState 以兼容未迁移的敌人
    public void SwitchState(EnemyState state)
    {
        if (UseFsm) return; // 使用 FSM 后不再调用此方法

        var newState = state switch
        {
            EnemyState.Patrol => patrolState,
            EnemyState.Chase  => chaseState,
            EnemyState.Attack => attackState,
            _ => null
        };
        if (newState == null) return;
        currentState?.OnExit();
        currentState = newState;
        currentState.OnEnter(this);
    }

    // ============================================================
    //  事件方法（新版通过 FSM 调用，旧版直接调用）
    // ============================================================

    public void OnTakeDamage(Transform attackTrans)
    {
        if (UseFsm)
        {
            _fsm.OnTakeDamage(attackTrans);
            return;
        }

        // ---- 旧版逻辑（迁移完成后可删除）----
        attacker = attackTrans;
        Debug.Log("发生翻转");
        if (attackTrans.position.x - transform.position.x > 0)
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        if (attackTrans.position.x - transform.position.x < 0)
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        isHurt = true;
        anim.SetTrigger("hurt");
        Vector2 dir = new Vector2(transform.position.x - attackTrans.position.x, 0).normalized;
        rb.velocity = new Vector2(0, rb.velocity.y);
        if (enemiesAudio != null && enemiesAudio.hurtAudio != null)
        {
            enemiesAudio.audioSource.clip = enemiesAudio.hurtAudio;
            enemiesAudio.audioSource.Play();
        }
        Debug.Log("播放" + gameObject.name + "的受伤音效");
        StartCoroutine(OnHurt(dir));
    }

    private IEnumerator OnHurt(Vector2 dir)
    {
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.45f);
        isHurt = false;
    }

    public void OnDie()
    {
        if (UseFsm)
        {
            _fsm.OnDie();
            return;
        }

        // ---- 旧版逻辑（迁移完成后可删除）----
        gameObject.layer = 2;
        currentSpeed = 0;
        anim.SetBool("dead", true);
        isDead = true;
        if (enemiesAudio != null && enemiesAudio.deadAudio != null)
        {
            enemiesAudio.audioSource.clip = enemiesAudio.deadAudio;
            enemiesAudio.audioSource.Play();
        }
        Debug.Log(this.gameObject.name + "当前对象的死亡音效播放");
        StartCoroutine(DestroyObject());
    }

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

    IEnumerator DestroyObject()
    {
        Debug.Log("销毁死亡的物体");
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}