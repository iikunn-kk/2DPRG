using System.Collections;
using UnityEngine;

/// <summary>
/// 通用敌人有限状态机。
/// 通过 SwitchTo() 统一切换状态，保证状态互斥。
///
/// 朝向约定（v2.1 统一）：
///   faceDir.x = 1 → 向右移动；faceDir.x = -1 → 向左移动
///   transform.localScale.x 的符号 = 贴图朝向（正数=默认方向）
///   巡逻时：faceDir.x = -localScale.x（贴图和移动反向）
///   追击/攻击时：通过 FaceTarget() 统一面向玩家
///
/// 修复记录（2026-05-29）：
///   - Bug #1: Chase 无玩家朝向 → 添加 FaceTarget()
///   - Bug #2: faceDir/scale 不一致 → 统一 FaceToTargetFaceDir 约定
///   - Bug #3: _isWaiting 跨状态泄漏 → ExitState(Patrol) 清零
///   - Bug #4: OnTakeDamage 不更新 faceDir → 补充同步
///   - Bug #5: Attack 状态 PatrolMove 仍执行 → FixedUpdate 排除 Attack
///   - Bug #6: 攻击首帧双发 → _attackCooldownRemaining 初始化为正延迟
///   - Bug #7: Chase 碰墙翻转约定混乱 → 使用一致的反转逻辑
///   - Bug #8: FoundPlayer BoxCast 方向 → 使用 faceDir（修正后一致）
/// </summary>
public class EnemyFsm : MonoBehaviour
{
    [Header("配置（拖入对应的 EnemyConfig_SO 资产）")]
    public EnemyConfig_SO config;

    [Header("组件引用（自动获取）")]
    public Enemy enemy;
    public Rigidbody2D rb;
    public Animator anim;
    public PhysicsCheck physicsCheck;
    public EnemiesAudio enemiesAudio;

    [Header("朝向设置")]
    [Tooltip("精灵默认朝向：勾选表示朝右，不勾选表示朝左")]
    public bool spriteFacesRight = false;  // 你的狼是朝左的，保持默认 false

    // ---------- 运行时状态 ----------
    private EnemyFsmState _currentState;
    private Coroutine _hurtCoroutine;
    private float _lostTimeCounter;
    private float _waitTimeCounter;
    private bool _isWaiting;

    // ---------- 攻击冷却 ----------
    private float _attackCooldownRemaining;

    // ---------- 缓存的 WaitForSeconds ----------
    private static readonly WaitForSeconds HurtWait = new WaitForSeconds(0.45f);
    private static readonly WaitForSeconds DestroyDelay = new WaitForSeconds(1f);

    // ---------- 属性 ----------
    public EnemyFsmState CurrentState => _currentState;

    // 缓存的玩家 Transform（用于朝向计算，减少 Find 调用）
    private Transform _playerTransform;

    // ============================================================
    //  Unity 生命周期
    // ============================================================

    private void Awake()
    {
        if (enemy == null) enemy = GetComponent<Enemy>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (anim == null) anim = GetComponent<Animator>();
        if (physicsCheck == null) physicsCheck = GetComponent<PhysicsCheck>();
        if (enemiesAudio == null) enemiesAudio = GetComponent<EnemiesAudio>();

        _waitTimeCounter = config != null ? config.waitTime : 2f;
    }

    private void OnEnable()
    {
        CachePlayerTransform();
        SwitchTo(EnemyFsmState.Patrol);
    }

    private void Update()
    {
        switch (_currentState)
        {
            case EnemyFsmState.Patrol: UpdatePatrol(); break;
            case EnemyFsmState.Chase:  UpdateChase();  break;
            case EnemyFsmState.Attack: UpdateAttack(); break;
            case EnemyFsmState.Hurt:   break;
            case EnemyFsmState.Dead:   break;
        }

        if (enemy != null && enemy.flipTimer > 0)
            enemy.flipTimer -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        // Bug #5 修复：Attack 状态不移动
        if (_currentState == EnemyFsmState.Hurt ||
            _currentState == EnemyFsmState.Dead ||
            _currentState == EnemyFsmState.Attack)
            return;

        if (_currentState == EnemyFsmState.Patrol && _isWaiting)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        PatrolMove();
    }

    // ============================================================
    //  状态切换
    // ============================================================

    public void SwitchTo(EnemyFsmState newState)
    {
        if (_currentState == EnemyFsmState.Dead && newState != EnemyFsmState.Dead)
            return;

        ExitState(_currentState);
        _currentState = newState;
        EnterState(_currentState);
    }

    // ============================================================
    //  Enter / Exit（Bug #3 修复：_isWaiting 清零）
    // ============================================================

    private void EnterState(EnemyFsmState state)
    {
        switch (state)
        {
            case EnemyFsmState.Patrol:
                if (config != null) enemy.currentSpeed = config.normalSpeed;
                // 首次巡逻：从当前朝向计算 faceDir
                enemy.faceDir = new Vector3(-transform.localScale.x, 0, 0);
                _isWaiting = false;               // Bug #3: 清理残留
                _waitTimeCounter = config != null ? config.waitTime : 2f;
                anim?.SetBool("walk", true);
                break;

            case EnemyFsmState.Chase:
                if (config != null) enemy.currentSpeed = config.chaseSpeed;
                _lostTimeCounter = config != null ? config.lostTime : 3f;
                _isWaiting = false;               // Bug #3: 清理残留
                FaceTarget();                     // Bug #1: 进入追击时面向玩家
                anim?.SetBool("run", true);
                break;

            case EnemyFsmState.Attack:
                FaceTarget();                     // Bug #1: 攻击时面向玩家
                anim?.SetTrigger("attack");
                _attackCooldownRemaining = config != null ? config.attackRate : 2f;  // Bug #6: 首帧不双发
                break;

            case EnemyFsmState.Hurt:
                anim?.SetTrigger("hurt");
                enemy.isHurt = true;
                if (enemiesAudio != null && enemiesAudio.hurtAudio != null)
                {
                    enemiesAudio.audioSource.clip = enemiesAudio.hurtAudio;
                    enemiesAudio.audioSource.Play();
                }
                break;

            case EnemyFsmState.Dead:
                anim?.SetBool("dead", true);
                enemy.isDead = true;
                enemy.currentSpeed = 0;
                gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                if (enemiesAudio != null && enemiesAudio.deadAudio != null)
                {
                    enemiesAudio.audioSource.clip = enemiesAudio.deadAudio;
                    enemiesAudio.audioSource.Play();
                }
                break;
        }
    }

    private void ExitState(EnemyFsmState state)
    {
        switch (state)
        {
            case EnemyFsmState.Patrol:
                anim?.SetBool("walk", false);
                _isWaiting = false;               // Bug #3: 防止泄漏到后续状态
                break;

            case EnemyFsmState.Chase:
                anim?.SetBool("run", false);
                break;

            case EnemyFsmState.Attack:
                // 攻击退出无需特殊处理
                break;

            case EnemyFsmState.Hurt:
                enemy.isHurt = false;
                break;

            case EnemyFsmState.Dead:
                break;
        }
    }

    // ============================================================
    //  各状态 Update 逻辑
    // ============================================================

    private void UpdatePatrol()
    {
        if (FoundPlayer())
        {
            SwitchTo(EnemyFsmState.Chase);
            return;
        }

        // 墙检测
        if (enemy.flipTimer <= 0)
        {
            if (IsTouchingWall())
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

        // 等待计时 → 翻转朝向继续巡逻
        if (_isWaiting)
        {
            _waitTimeCounter -= Time.deltaTime;
            if (_waitTimeCounter <= 0)
            {
                _isWaiting = false;
                _waitTimeCounter = config != null ? config.waitTime : 2f;

                // Bug #7 修复：统一翻转逻辑
                float flipScaleX = -transform.localScale.x;
                transform.localScale = new Vector3(flipScaleX, transform.localScale.y, transform.localScale.z);
                // faceDir = -localScale.x（巡逻约定）
                enemy.faceDir = new Vector3(-flipScaleX, 0, 0);
            }
        }
    }

    private void UpdateChase()
    {
        // 丢失玩家 → 回 Patrol
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
            FaceTarget();                     // Bug #1: 每帧面向玩家
        }

        // 进攻击范围 → Attack
        if (config != null && config.hasAttackState && FoundPlayerInAttackRange())
        {
            SwitchTo(EnemyFsmState.Attack);
            return;
        }

        // 碰墙 → 转向（Bug #7 修复）
        if (enemy.flipTimer <= 0 && IsTouchingWall())
        {
            FlipDirection();
            enemy.flipTimer = config != null ? config.wallFlipDelay : 0.2f;
        }
    }

    private void UpdateAttack()
    {
        _attackCooldownRemaining -= Time.deltaTime;
        if (_attackCooldownRemaining <= 0)
        {
            if (FoundPlayerInAttackRange())
            {
                FaceTarget();                 // Bug #1: 每次攻击前确认朝向
                anim?.SetTrigger("attack");
                _attackCooldownRemaining = config != null ? config.attackRate : 2f;
            }
            else
            {
                SwitchTo(EnemyFsmState.Chase);
            }
        }
    }

    // ============================================================
    //  移动
    // ============================================================

    private void PatrolMove()
    {
        if (_isWaiting) return;
        rb.velocity = new Vector2(enemy.currentSpeed * enemy.faceDir.x, rb.velocity.y);
    }

    // ============================================================
    //  朝向管理（Bug #1 #2 #4 #7 统一修复）
    // ============================================================

    /// <summary>
    /// 使敌人面朝玩家（Chase/Attack 状态使用）。
    /// 通过 spriteFacesRight 配置适配不同的美术资源朝向。
    /// </summary>
    private void FaceTarget()
    {
        CachePlayerTransform();
        if (_playerTransform == null) return;

        float dir = _playerTransform.position.x - transform.position.x;
        float absScale = Mathf.Abs(transform.localScale.x);
        bool playerOnRight = dir > 0;

        // 设置贴图朝向
        if (playerOnRight)
        {
            // spriteFacesRight=true  → scale>0 朝右，保留正数
            // spriteFacesRight=false → scale>0 朝左，需要翻转
            float sx = spriteFacesRight ? absScale : -absScale;
            transform.localScale = new Vector3(sx, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            float sx = spriteFacesRight ? -absScale : absScale;
            transform.localScale = new Vector3(sx, transform.localScale.y, transform.localScale.z);
        }

        // 移动方向始终指向玩家（追击/攻击用）
        enemy.faceDir = new Vector3(playerOnRight ? 1 : -1, 0, 0);
    }

    /// <summary>
    /// 翻转方向（碰墙时调用）。
    /// 翻转 scale 符号并同步 faceDir。
    /// </summary>
    private void FlipDirection()
    {
        float newScaleX = -transform.localScale.x;
        transform.localScale = new Vector3(newScaleX, transform.localScale.y, transform.localScale.z);
        // faceDir 跟随：巡逻时 faceDir = -localScale.x，追击时 faceDir 指向玩家
        enemy.faceDir = new Vector3(-newScaleX, 0, 0);
    }

    private void CachePlayerTransform()
    {
        if (_playerTransform == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go != null) _playerTransform = go.transform;
        }
    }

    // ============================================================
    //  事件方法
    // ============================================================

    public void OnTakeDamage(Transform attacker)
    {
        if (_currentState == EnemyFsmState.Dead) return;

        // Bug #4 修复：统一朝向逻辑，适配 spriteFacesRight
        if (attacker != null)
        {
            float dir = attacker.position.x - transform.position.x;
            float absScale = Mathf.Abs(transform.localScale.x);
            if (dir > 0)
            {
                float sx = spriteFacesRight ? absScale : -absScale;
                transform.localScale = new Vector3(sx, transform.localScale.y, transform.localScale.z);
            }
            else
            {
                float sx = spriteFacesRight ? -absScale : absScale;
                transform.localScale = new Vector3(sx, transform.localScale.y, transform.localScale.z);
            }
            enemy.faceDir = new Vector3(Mathf.Sign(transform.localScale.x), 0, 0);
        }

        enemy.attacker = attacker;

        SwitchTo(EnemyFsmState.Hurt);

        if (_hurtCoroutine != null) StopCoroutine(_hurtCoroutine);
        _hurtCoroutine = StartCoroutine(HurtRoutine(attacker));
    }

    private IEnumerator HurtRoutine(Transform attacker)
    {
        Vector2 dir = attacker != null
            ? new Vector2(transform.position.x - attacker.position.x, 0).normalized
            : Vector2.right;

        rb.velocity = new Vector2(0, rb.velocity.y);
        float force = config != null ? config.hurtForce : 5f;
        rb.AddForce(dir * force, ForceMode2D.Impulse);

        yield return HurtWait;

        if (_currentState == EnemyFsmState.Hurt)
        {
            if (FoundPlayer())
                SwitchTo(EnemyFsmState.Chase);
            else
                SwitchTo(EnemyFsmState.Patrol);
        }
        _hurtCoroutine = null;
    }

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

    #region 对象池 + 检测

    [Header("对象池")]
    public bool usePool = true;

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
        _playerTransform = null;

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
            rb.velocity = Vector2.zero;

        gameObject.layer = LayerMask.NameToLayer("Enemy");
    }

    /// <summary>
    /// BoxCast 检测前方是否有玩家（Bug #8 修复：使用 faceDir 作为检测方向）。
    /// </summary>
    public bool FoundPlayer()
    {
        if (enemy == null) return false;
        return Physics2D.BoxCast(
            transform.position + (Vector3)enemy.centerOffset,
            enemy.checkSize, 0,
            enemy.faceDir,
            enemy.checkDistance,
            enemy.attackLayer
        );
    }

    public bool FoundPlayerInAttackRange()
    {
        if (config == null || enemy == null) return false;
        return Physics2D.OverlapCircle(
            transform.position,
            config.attackRange,
            enemy.attackLayer
        ) != null;
    }

    private bool IsTouchingWall()
    {
        return physicsCheck.touchLeftWall || physicsCheck.touchRightWall ||
               physicsCheck.touchLeftAirWall || physicsCheck.touchRightAirWall;
    }

    #endregion
}
