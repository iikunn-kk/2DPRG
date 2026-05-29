using System.Collections;
using UnityEngine;

/// <summary>
/// 通用敌人有限状态机。
/// 取代所有独立的 *PatrolState / *ChaseState / *AttackState 脚本。
/// 通过 SwitchTo() 统一切换状态，保证状态互斥。
/// Hurt 和 Dead 是正式状态，支持打断优先级。
/// </summary>
public class EnemyFsm : MonoBehaviour
{
    [Header("配置（拖入对应的 EnemyConfig_SO 资产）")]
    public EnemyConfig_SO config;

    [Header("组件引用（自动获取）")]
    public Enemy enemy;          // 敌人基类引用
    public Rigidbody2D rb;
    public Animator anim;
    public PhysicsCheck physicsCheck;
    public EnemiesAudio enemiesAudio;

    // ---------- 运行时状态 ----------
    private EnemyFsmState _currentState;
    private Coroutine _hurtCoroutine;
    private float _lostTimeCounter;
    private float _waitTimeCounter;
    private bool _isWaiting;

    // ---------- 攻击冷却 ----------
    private float _attackCooldownRemaining;

    // ---------- 缓存的 WaitForSeconds（避免 GC 分配）----------
    private static readonly WaitForSeconds HurtWait = new WaitForSeconds(0.45f);
    private static readonly WaitForSeconds DestroyDelay = new WaitForSeconds(1f);

    // ---------- 属性 ----------
    public EnemyFsmState CurrentState => _currentState;

    // ============================================================
    //  Unity 生命周期
    // ============================================================

    private void Awake()
    {
        // 自动获取组件（如果 Inspector 未手动拖入）
        if (enemy == null) enemy = GetComponent<Enemy>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (anim == null) anim = GetComponent<Animator>();
        if (physicsCheck == null) physicsCheck = GetComponent<PhysicsCheck>();
        if (enemiesAudio == null) enemiesAudio = GetComponent<EnemiesAudio>();

        _waitTimeCounter = config != null ? config.waitTime : 2f;
    }

    private void OnEnable()
    {
        SwitchTo(EnemyFsmState.Patrol);
    }

    private void Update()
    {
        // 逻辑帧更新（各状态的 LogicUpdate 逻辑内联至此）
        switch (_currentState)
        {
            case EnemyFsmState.Patrol: UpdatePatrol(); break;
            case EnemyFsmState.Chase: UpdateChase(); break;
            case EnemyFsmState.Attack: UpdateAttack(); break;
            case EnemyFsmState.Hurt:   /* Hurt 由协程驱动，Update 不做逻辑 */ break;
            case EnemyFsmState.Dead:   /* Dead 是终态 */ break;
        }

        // 翻转计时器递减
        if (enemy != null && enemy.flipTimer > 0)
        {
            enemy.flipTimer -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        // 死亡和受击时不允许移动（修复原代码按位与 & bug，改为明确状态判断）
        if (_currentState != EnemyFsmState.Hurt && _currentState != EnemyFsmState.Dead)
        {
            // 巡逻时的等待拦截由 UpdatePatrol 内部处理
            if (_currentState == EnemyFsmState.Patrol && _isWaiting)
            {
                // 等待中，不动
                rb.velocity = new Vector2(0, rb.velocity.y);
                return;
            }
            PatrolMove(); // Patrol 和 Chase 共用移动逻辑（速度由进入状态时设置）
        }

        // 物理帧更新
        switch (_currentState)
        {
            case EnemyFsmState.Patrol: /* PhysicsUpdate 无逻辑 */ break;
            case EnemyFsmState.Chase:  /* PhysicsUpdate 无逻辑 */ break;
            case EnemyFsmState.Attack: /* PhysicsUpdate 无逻辑 */ break;
        }
    }

    private void OnDisable()
    {
        // 清理协程
        if (_hurtCoroutine != null)
        {
            StopCoroutine(_hurtCoroutine);
            _hurtCoroutine = null;
        }
    }

    // ============================================================
    //  状态切换（唯一入口，保证互斥）
    // ============================================================

    /// <summary>
    /// 切换状态。所有状态转换必须通过此方法，禁止直接修改 _currentState。
    /// </summary>
    public void SwitchTo(EnemyFsmState newState)
    {
        // 死亡后不再切换任何状态
        if (_currentState == EnemyFsmState.Dead && newState != EnemyFsmState.Dead)
            return;

        ExitState(_currentState);
        _currentState = newState;
        EnterState(_currentState);
    }

    // ============================================================
    //  Enter / Exit
    // ============================================================

    private void EnterState(EnemyFsmState state)
    {
        switch (state)
        {
            case EnemyFsmState.Patrol:
                if (config != null) enemy.currentSpeed = config.normalSpeed;
                // 初始化 faceDir：根据当前朝向确定移动方向（确保首次巡逻时能动）
                enemy.faceDir = new Vector3(-transform.localScale.x, 0, 0);
                anim?.SetBool("walk", true);
                Debug.Log($"[{gameObject.name}] 进入 Patrol 状态");
                break;

            case EnemyFsmState.Chase:
                if (config != null) enemy.currentSpeed = config.chaseSpeed;
                anim?.SetBool("run", true);
                _lostTimeCounter = config != null ? config.lostTime : 3f;
                Debug.Log($"[{gameObject.name}] 进入 Chase 状态");
                break;

            case EnemyFsmState.Attack:
                anim?.SetTrigger("attack"); // 假设攻击动画用 Trigger 触发
                _attackCooldownRemaining = 0f; // 立即攻击
                Debug.Log($"[{gameObject.name}] 进入 Attack 状态");
                break;

            case EnemyFsmState.Hurt:
                anim?.SetTrigger("hurt");
                enemy.isHurt = true; // 保留字段以兼容现有动画事件
                if (enemiesAudio != null && enemiesAudio.hurtAudio != null)
                {
                    enemiesAudio.audioSource.clip = enemiesAudio.hurtAudio;
                    enemiesAudio.audioSource.Play();
                }
                Debug.Log($"[{gameObject.name}] 进入 Hurt 状态");
                break;

            case EnemyFsmState.Dead:
                anim?.SetBool("dead", true);
                enemy.isDead = true;
                enemy.currentSpeed = 0;
                gameObject.layer = LayerMask.NameToLayer("Ignore Raycast"); // 避免继续交互
                if (enemiesAudio != null && enemiesAudio.deadAudio != null)
                {
                    enemiesAudio.audioSource.clip = enemiesAudio.deadAudio;
                    enemiesAudio.audioSource.Play();
                }
                Debug.Log($"[{gameObject.name}] 进入 Dead 状态");
                break;
        }
    }

    private void ExitState(EnemyFsmState state)
    {
        switch (state)
        {
            case EnemyFsmState.Patrol:
                anim?.SetBool("walk", false);
                Debug.Log($"[{gameObject.name}] 退出 Patrol 状态");
                break;

            case EnemyFsmState.Chase:
                anim?.SetBool("run", false);
                Debug.Log($"[{gameObject.name}] 退出 Chase 状态");
                break;

            case EnemyFsmState.Attack:
                // Attack 退出无特殊处理
                Debug.Log($"[{gameObject.name}] 退出 Attack 状态");
                break;

            case EnemyFsmState.Hurt:
                enemy.isHurt = false;
                Debug.Log($"[{gameObject.name}] 退出 Hurt 状态");
                break;

            case EnemyFsmState.Dead:
                // Dead 是终态，不应被退出；此处保留以防扩展
                break;
        }
    }

    // ============================================================
    //  各状态 Update 逻辑
    // ============================================================

    private void UpdatePatrol()
    {
        // 检测是否发现玩家 → 切换到 Chase
        if (FoundPlayer())
        {
            SwitchTo(EnemyFsmState.Chase);
            return;
        }

        // 墙检测（翻转计时器 > 0 时跳过）
        if (enemy.flipTimer <= 0)
        {
            if (physicsCheck.touchLeftWall || physicsCheck.touchRightWall ||
                physicsCheck.touchLeftAirWall || physicsCheck.touchRightAirWall)
            {
                _isWaiting = true;
                anim?.SetBool("walk", false);
                rb.velocity = new Vector2(0, rb.velocity.y);
                enemy.flipTimer = config != null ? config.wallFlipDelay : 0.2f;
            }
            else
            {
                anim?.SetBool("walk", true);
            }
        }

        // 等待计时
        if (_isWaiting)
        {
            _waitTimeCounter -= Time.deltaTime;
            if (_waitTimeCounter <= 0)
            {
                _isWaiting = false;
                _waitTimeCounter = config != null ? config.waitTime : 2f;
                // 翻转朝向
                float faceX = -transform.localScale.x;
                transform.localScale = new Vector3(faceX, transform.localScale.y, transform.localScale.z);
                enemy.faceDir = new Vector3(-transform.localScale.x, 0, 0);
            }
        }
    }

    private void UpdateChase()
    {
        // 丢失玩家 → 倒计时结束后切回 Patrol
        if (!FoundPlayer())
        {
            _lostTimeCounter -= Time.deltaTime;
            if (_lostTimeCounter <= 0)
            {
                SwitchTo(EnemyFsmState.Patrol);
                return;
            }
        }
        else
        {
            _lostTimeCounter = config != null ? config.lostTime : 3f;
        }

        // 攻击范围内且有攻击状态 → 切换到 Attack
        if (config != null && config.hasAttackState && FoundPlayerInAttackRange())
        {
            SwitchTo(EnemyFsmState.Attack);
            return;
        }

        // 墙检测（追击状态直接翻转，不等待）
        if (enemy.flipTimer <= 0)
        {
            if (physicsCheck.touchLeftWall || physicsCheck.touchRightWall ||
                physicsCheck.touchLeftAirWall || physicsCheck.touchRightAirWall)
            {
                transform.localScale = new Vector3(enemy.faceDir.x, transform.localScale.y, transform.localScale.z);
                enemy.flipTimer = config != null ? config.wallFlipDelay : 0.2f;
            }
        }
    }

    private void UpdateAttack()
    {
        // 攻击动画播放期间等待动画事件或计时结束
        _attackCooldownRemaining -= Time.deltaTime;
        if (_attackCooldownRemaining <= 0)
        {
            // 攻击间隔结束，判断下一步行动
            if (FoundPlayerInAttackRange())
            {
                // 仍在攻击范围内 → 继续攻击
                anim?.SetTrigger("attack");
                _attackCooldownRemaining = config != null ? config.attackRate : 2f;
            }
            else
            {
                // 离开攻击范围 → 回到 Chase
                SwitchTo(EnemyFsmState.Chase);
            }
        }
    }

    // ============================================================
    //  移动
    // ============================================================

    /// <summary>
    /// 巡逻/追击 共用移动：按 faceDir 方向移动。
    /// </summary>
    private void PatrolMove()
    {
        if (_isWaiting) return; // 等待中不移动
        rb.velocity = new Vector2(enemy.currentSpeed * enemy.faceDir.x, rb.velocity.y);
    }

    // ============================================================
    //  事件方法（由外部调用，如 Character.cs 的 TakeDamage）
    // ============================================================

    /// <summary>
    /// 受击入口。由 Enemy.OnTakeDamage 调用。
    /// </summary>
    public void OnTakeDamage(Transform attacker)
    {
        // 已死亡则不再受击
        if (_currentState == EnemyFsmState.Dead) return;

        // 转向攻击者
        if (attacker.position.x > transform.position.x)
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);

        enemy.attacker = attacker;

        // 切换到 Hurt 状态（会打断当前状态）
        SwitchTo(EnemyFsmState.Hurt);

        // 启动受击协程（击退 → 恢复）
        if (_hurtCoroutine != null) StopCoroutine(_hurtCoroutine);
        _hurtCoroutine = StartCoroutine(HurtRoutine(attacker));
    }

    private IEnumerator HurtRoutine(Transform attacker)
    {
        // 击退
        Vector2 dir = new Vector2(transform.position.x - attacker.position.x, 0).normalized;
        rb.velocity = new Vector2(0, rb.velocity.y);
        float force = config != null ? config.hurtForce : 5f;
        rb.AddForce(dir * force, ForceMode2D.Impulse);

        yield return HurtWait;

        // 恢复 → 根据是否发现玩家决定下一个状态
        if (_currentState == EnemyFsmState.Hurt) // 防止期间死亡
        {
            if (FoundPlayer())
                SwitchTo(EnemyFsmState.Chase);
            else
                SwitchTo(EnemyFsmState.Patrol);
        }
        _hurtCoroutine = null;
    }

    /// <summary>
    /// 是否启用对象池模式（死亡时不销毁，而是停用放回池中）
    /// </summary>
    [Header("对象池")]
    public bool usePool = true;

    /// <summary>
    /// 死亡入口。由 Enemy.OnDie 调用。
    /// </summary>
    public void OnDie()
    {
        if (_currentState == EnemyFsmState.Dead) return;
        SwitchTo(EnemyFsmState.Dead);
        StartCoroutine(AfterDeathRoutine());
    }

    private IEnumerator AfterDeathRoutine()
    {
        yield return DestroyDelay;

        if (usePool)
        {
            ResetToPoolReady();
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 重置为对象池就绪状态（在对象被重新激活时由 OnEnable 自动进入 Patrol）
    /// </summary>
    private void ResetToPoolReady()
    {
        if (_hurtCoroutine != null)
        {
            StopCoroutine(_hurtCoroutine);
            _hurtCoroutine = null;
        }

        _currentState = EnemyFsmState.Patrol;
        _isWaiting = false;
        _attackCooldownRemaining = 0f;
        _lostTimeCounter = 0f;
        _waitTimeCounter = config != null ? config.waitTime : 2f;

        if (enemy != null)
        {
            enemy.isHurt = false;
            enemy.isDead = false;
            enemy.currentSpeed = config != null ? config.normalSpeed : enemy.normalSpeed;
            enemy.faceDir = new Vector3(-transform.localScale.x, 0, 0);
            enemy.attacker = null;
        }

        if (anim != null)
        {
            anim.SetBool("walk", false);
            anim.SetBool("run", false);
            anim.SetBool("dead", false);
            anim.Rebind();
            anim.Update(0f);
        }

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        gameObject.layer = LayerMask.NameToLayer("Enemy");
    }

    // ============================================================
    //  检测工具
    // ============================================================

    /// <summary>
    /// 检测前方是否有玩家（使用 Enemy.FoundPlayer 的 BoxCast 逻辑）。
    /// </summary>
    public bool FoundPlayer()
    {
        if (enemy == null) return false;
        return Physics2D.BoxCast(
            transform.position + (Vector3)enemy.centerOffset,
            enemy.checkSize,
            0,
            enemy.faceDir,
            enemy.checkDistance,
            enemy.attackLayer
        );
    }

    /// <summary>
    /// 检测玩家是否在攻击范围内（圆形检测）。
    /// </summary>
    public bool FoundPlayerInAttackRange()
    {
        if (config == null) return false;
        return Physics2D.OverlapCircle(
            transform.position,
            config.attackRange,
            enemy.attackLayer
        ) != null;
    }
}
