using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// 玩家控制器。
/// 使用 PlayerState 枚举 + SwitchTo() 保证状态互斥，取代原来的 7 个无约束 bool。
/// </summary>
public class PlayerController : Singleton<PlayerController>
{
    [Header("土狼时间 & 输入缓冲")]
    [SerializeField] float coyoteTime = 0.1f;   // 离开平台后仍可跳跃的时间窗口
    [SerializeField] float jumpBufferTime = 0.15f; // 提前按跳跃的缓存时间
    private float _coyoteTimeCounter;             // 土狼时间倒计时
    private float _jumpBufferCounter;             // 跳跃缓冲倒计时
    private bool _hasJumpBuffer;                  // 是否有待执行的缓冲跳跃

    [Header("二段跳计数器")]
    public float maximumNumberJumps;
    public float currentNumberJumps;

    private CharacterStats characterStats;

    [Header("基本参数")]
    public float hurtForce;
    public float currentSpeed;
    public float jumpForce;
    public float slideDistance;
    public float slideSpeed;
    public int slidePowerCost;
    public float tempX;
    public float faceDir;
    private float slideStartPos;
    private float currentSlideDirection;

    private Vector2 originalOffset;
    private Vector2 originalSize;

    [Header("物理材质")]
    public PhysicsMaterial2D normal;
    public PhysicsMaterial2D wall;

    private PlayerInputController inputControl;
    private Rigidbody2D rb;
    private CapsuleCollider2D coll;
    private SpriteRenderer sr;
    private PlayerAnimation playerAnimation;
    private PhysicsCheck physicsCheck;
    private Character character;
    public Vector2 inputDirection;

    [Header("音效")]
    private PlayerAudio playerAudio;
    public UnityEvent PlayRunAudio;
    public UnityEvent StopRunAudio;

    // ========== 状态机（取代所有 bool）==========
    private PlayerState _state;
    private Coroutine _slideCoroutine;
    private Coroutine _jumpResetCoroutine;
    private bool _wasGrounded;                // 上一帧是否在地面（用于检测落地瞬间）

    // ============================================================
    //  Unity 生命周期
    // ============================================================

    protected override void Awake()
    {
        base.Awake();

        inputControl = new PlayerInputController();
        characterStats = GetComponent<CharacterStats>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        physicsCheck = GetComponent<PhysicsCheck>();
        coll = GetComponent<CapsuleCollider2D>();
        playerAnimation = GetComponent<PlayerAnimation>();
        playerAudio = GetComponent<PlayerAudio>();
        character = GetComponent<Character>();

        inputControl.Gameplay.Jump.started += Jump;
        inputControl.Gameplay.Attack.started += PlayerAttack;
        inputControl.Gameplay.Slide.started += Slide;

        originalOffset = coll.offset;
        originalSize = coll.size;
        tempX = transform.localScale.x;
        faceDir = 1;
        currentNumberJumps = 0;

        // 初始状态
        _state = PlayerState.Idle;
    }

    private void OnEnable()
    {
        inputControl.Enable();
        GameManager.Instance.RegisterPlayer(characterStats);
        Debug.Log("注册成功");
    }

    void Start()
    {
        SaveManager.Instance.LoadPlayerData();
        Debug.Log("人物数据加完毕");
    }

    private void OnDisable()
    {
        if (inputControl != null)
        {
            inputControl.Disable();
        }
    }

    private void Update()
    {
        inputDirection = inputControl.Gameplay.Move.ReadValue<Vector2>();

        // 更新面朝方向（从输入读取）
        if (inputDirection.x > 0) faceDir = 1;
        if (inputDirection.x < 0) faceDir = -1;

        // ===== 土狼时间倒计时 =====
        if (_coyoteTimeCounter > 0)
        {
            _coyoteTimeCounter -= Time.deltaTime;
            if (_coyoteTimeCounter <= 0)
                _coyoteTimeCounter = 0;
        }

        // ===== 跳跃缓冲倒计时 =====
        if (_hasJumpBuffer)
        {
            _jumpBufferCounter -= Time.deltaTime;
            if (_jumpBufferCounter <= 0)
            {
                _hasJumpBuffer = false;
                _jumpBufferCounter = 0;
            }
        }

        // ===== 落地瞬间：重置跳跃次数 + 执行缓冲跳跃 =====
        if (physicsCheck.isGround)
        {
            // 仅从空中回到地面的那一帧才重置（避免每帧清零导致二段跳被吞）
            if (!_wasGrounded)
            {
                currentNumberJumps = 0;
                _coyoteTimeCounter = 0;

                // Input Buffer：落地时检查是否有缓存的跳跃
                if (_hasJumpBuffer && _state != PlayerState.Jump)
                {
                    _hasJumpBuffer = false;
                    ExecuteJump();
                }
            }
            _wasGrounded = true;
        }
        else
        {
            _wasGrounded = false;
        }

        CheckState();
        UpdateMovementState();
        UpdateCrouchState();
    }

    private void FixedUpdate()
    {
        // 只有非 Hurt/Dialogue/Dead 状态才允许移动
        if (_state != PlayerState.Hurt &&
            _state != PlayerState.Dead &&
            _state != PlayerState.Dialogue)
        {
            Move();
        }
    }

    // ============================================================
    //  状态切换（唯一入口，保证互斥）
    // ============================================================

    /// <summary>
    /// 切换到新状态。处理优先级：Dead > Hurt > Dialogue > 其他。
    /// </summary>
    public void SwitchTo(PlayerState newState)
    {
        if (_state == PlayerState.Dead && newState != PlayerState.Dead)
            return;

        if (_state == PlayerState.Hurt &&
            (newState == PlayerState.Attack ||
             newState == PlayerState.Jump ||
             newState == PlayerState.Slide))
            return;

        PlayerState oldState = _state;

        ExitState(oldState, newState);
        _state = newState;
        EnterState(newState, oldState);
    }

    private void EnterState(PlayerState state, PlayerState fromState)
    {
        switch (state)
        {
            case PlayerState.Idle:
                break;
            case PlayerState.Run:
                PlayRunAudio?.Invoke();
                break;
            case PlayerState.Jump:
                isJumping = true;
                playerAudio?.PlayWithJump();
                break;
            case PlayerState.Fall:
                break;
            case PlayerState.Crouch:
                isCrouch = true;
                break;
            case PlayerState.Attack:
                isAttack = true;
                playerAnimation?.PlayAttack();
                break;
            case PlayerState.Slide:
                isSlide = true;
                playerAudio?.PlaySlideSound();
                break;
            case PlayerState.Hurt:
                isHurt = true;
                playerAnimation?.PlayHurt();
                break;
            case PlayerState.Dead:
                isDead = true;
                inputControl.Gameplay.Disable();
                PausePanel.Instance?.DeadToRestartGame();
                break;
            case PlayerState.Dialogue:
                isDialogue = true;
                break;
        }
    }

    private void ExitState(PlayerState state, PlayerState toState)
    {
        switch (state)
        {
            case PlayerState.Run:
                StopRunAudio?.Invoke();
                break;
            case PlayerState.Jump:
                isJumping = false; // 清理旧字段（保留以兼容动画事件）
                break;
            case PlayerState.Attack:
                isAttack = false;
                break;
            case PlayerState.Slide:
                isSlide = false;
                gameObject.layer = LayerMask.NameToLayer("Player");
                break;
            case PlayerState.Hurt:
                isHurt = false;
                break;
            case PlayerState.Dialogue:
                isDialogue = false;
                break;
            case PlayerState.Crouch:
                isCrouch = false;
                break;
        }
    }

    // ============================================================
    //  移动
    // ============================================================

    public void Move()
    {
        rb.velocity = new Vector2(inputDirection.x * currentSpeed, rb.velocity.y);
        transform.localScale = new Vector3(tempX * faceDir, transform.localScale.y, transform.localScale.z);
    }

    /// <summary>
    /// 蹲下状态管理（每帧检测，保证即时响应）
    /// </summary>
    private void UpdateCrouchState()
    {
        if (_state == PlayerState.Dead || _state == PlayerState.Hurt) return;

        bool shouldCrouch = inputDirection.y < -0.5f && physicsCheck.isGround;

        if (shouldCrouch && _state != PlayerState.Crouch && _state != PlayerState.Slide)
        {
            SwitchTo(PlayerState.Crouch);
            coll.offset = new Vector2(-0.18f, 0.8303528f);
            coll.size = new Vector2(1.3125f, 1.761332f);
        }
        else if (!shouldCrouch && _state == PlayerState.Crouch)
        {
            // S 松开：恢复站立，根据 A/D 状态决定 Run 还是 Idle
            coll.size = originalSize;
            coll.offset = originalOffset;
            SwitchTo(Mathf.Abs(inputDirection.x) > 0.1f ? PlayerState.Run : PlayerState.Idle);
        }
    }

    // ============================================================
    //  输入回调
    // ============================================================

    private void Jump(InputAction.CallbackContext context)
    {
        if (_state == PlayerState.Dead || _state == PlayerState.Dialogue)
            return;

        // 能跳：直接执行
        if (CanJumpNow())
        {
            ExecuteJump();
            return;
        }

        // 不能跳但有可能即将落地：缓冲输入
        if (!physicsCheck.isGround && _state != PlayerState.Jump)
        {
            _jumpBufferCounter = jumpBufferTime;
            _hasJumpBuffer = true;
        }
    }

    /// <summary>
    /// 判断当前是否可以跳跃：
    ///   第一段（次数未消耗）→ 需要在地面或土狼时间窗口内
    ///   第二段及以后          → 空中自由跳（二段跳/多段跳）
    /// </summary>
    private bool CanJumpNow()
    {
        if (currentNumberJumps >= maximumNumberJumps)
            return false;

        // 已经是空中跳跃（二段跳及以上）：随时允许
        if (currentNumberJumps >= 1)
            return true;

        // 第一段跳跃：需要地面、土狼时间、或贴墙
        return physicsCheck.isGround || _coyoteTimeCounter > 0 || physicsCheck.onWall;
    }

    /// <summary>
    /// 执行跳跃（施加力 + 状态切换）。
    /// </summary>
    private void ExecuteJump()
    {
        if (_state == PlayerState.Slide)
        {
            SwitchTo(PlayerState.Jump);
            StopCoroutine("TriggerSlide");
        }

        currentNumberJumps++;
        _coyoteTimeCounter = 0;   // 消耗土狼时间
        _hasJumpBuffer = false;   // 消耗缓冲
        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        SwitchTo(PlayerState.Jump);

        if (_jumpResetCoroutine != null) StopCoroutine(_jumpResetCoroutine);
        _jumpResetCoroutine = StartCoroutine(ResetJumpingState());
    }

    private void PlayerAttack(InputAction.CallbackContext context)
    {
        if (_state == PlayerState.Dead || _state == PlayerState.Dialogue)
            return;

        SwitchTo(PlayerState.Attack);
    }

    private void Slide(InputAction.CallbackContext context)
    {
        if (_state == PlayerState.Dead || _state == PlayerState.Dialogue)
            return;

        if (_state == PlayerState.Slide) return;
        if (!physicsCheck.isGround) return;
        if (character.currentPower < slidePowerCost) return;

        SwitchTo(PlayerState.Slide);
        slideStartPos = transform.position.x;
        currentSlideDirection = transform.localScale.x;
        gameObject.layer = LayerMask.NameToLayer("Enemy");

        if (_slideCoroutine != null) StopCoroutine(_slideCoroutine);
        _slideCoroutine = StartCoroutine(TriggerSlide());

        character.OnSlide(slidePowerCost);
    }

    // ============================================================
    //  协程
    // ============================================================

    private IEnumerator TriggerSlide()
    {
        do
        {
            yield return null;
            if (!physicsCheck.isGround) break;

            // 方向改变时重置起点
            if (transform.localScale.x != currentSlideDirection)
            {
                currentSlideDirection = transform.localScale.x;
                slideStartPos = transform.position.x;
            }

            // 撞墙检测
            if ((physicsCheck.touchLeftWall && currentSlideDirection < 0f) ||
                (physicsCheck.touchRightWall && currentSlideDirection > 0f) ||
                (physicsCheck.touchLeftAirWall && currentSlideDirection < 0f) ||
                (physicsCheck.touchRightAirWall && currentSlideDirection > 0f))
            {
                break;
            }

            rb.MovePosition(new Vector2(transform.position.x + currentSlideDirection * slideSpeed, transform.position.y));
        } while (Mathf.Abs(slideStartPos + currentSlideDirection * slideDistance - transform.position.x) > 0.9f);

        // 滑铲结束
        if (_state == PlayerState.Slide)
            SwitchTo(PlayerState.Idle);
        gameObject.layer = LayerMask.NameToLayer("Player");
        _slideCoroutine = null;
    }

    private IEnumerator ResetJumpingState()
    {
        yield return new WaitForSeconds(0.1f);
        if (_state == PlayerState.Jump)
            SwitchTo(PlayerState.Fall);
    }

    // ============================================================
    //  事件方法（由外部调用）
    // ============================================================

    public void GetHurt(Transform attacker)
    {
        if (_state == PlayerState.Dead) return;

        SwitchTo(PlayerState.Hurt);
        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2(transform.position.x - attacker.position.x, 0).normalized;
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);

        // 受击后自动恢复
        StartCoroutine(RecoverFromHurt());
    }

    private IEnumerator RecoverFromHurt()
    {
        yield return new WaitForSeconds(0.45f);
        if (_state == PlayerState.Hurt)
        {
            SwitchTo(physicsCheck.isGround ? PlayerState.Idle : PlayerState.Fall);
        }
    }

    public void PlayerDead()
    {
        if (_state == PlayerState.Dead) return;
        SwitchTo(PlayerState.Dead);
    }

    /// <summary>
    /// 强制停止一切动作，回到 Idle 状态。
    /// 用于对话系统接管玩家控制前，先清除运动动量。
    /// </summary>
    public void ForceToIdle()
    {
        rb.velocity = Vector2.zero;

        // 重置所有动作 bool（绕过 SwitchTo 避免触发 Jump 的 Enter 音效等）
        isAttack = false;
        isCrouch = false;
        isHurt = false;
        isJumping = false;
        isSlide = false;
        isDead = false;

        _state = PlayerState.Idle;
    }

    public void OnDialogueStart()
    {
        if (_state == PlayerState.Dead) return;
        SwitchTo(PlayerState.Dialogue);
    }

    public void OnDialogueEnd()
    {
        if (_state == PlayerState.Dialogue)
            SwitchTo(PlayerState.Idle);
    }

    // ============================================================
    //  物理状态检测
    // ============================================================

    private void CheckState()
    {
        coll.sharedMaterial = physicsCheck.isGround ? normal : wall;

        if (physicsCheck.onWall)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
        }
    }

    /// <summary>
    /// 根据物理状态自动检测并切换 Idle/Run/Fall 状态。
    /// 仅当当前不是手动触发的动作状态（Attack/Slide/Hurt/Dead/Dialogue/Crouch）时才执行。
    /// </summary>
    private void UpdateMovementState()
    {
        // 动作状态由输入回调管理，不自动切换
        if (_state == PlayerState.Attack || _state == PlayerState.Slide ||
            _state == PlayerState.Hurt || _state == PlayerState.Dead ||
            _state == PlayerState.Dialogue || _state == PlayerState.Crouch)
            return;

        // Jump 状态由 Jump() 输入回调 + ResetJumpingState 协程管理
        if (_state == PlayerState.Jump)
        {
            // 跳跃过程中落地 → 切换到 Idle/Run
            if (physicsCheck.isGround && rb.velocity.y <= 0.5f)
            {
                SwitchTo(Mathf.Abs(rb.velocity.x) > 0.1f ? PlayerState.Run : PlayerState.Idle);
            }
            return;
        }

        if (physicsCheck.isGround)
        {
            if (Mathf.Abs(rb.velocity.x) > 0.1f)
            {
                if (_state != PlayerState.Run)
                    SwitchTo(PlayerState.Run);
            }
            else
            {
                if (_state != PlayerState.Idle)
                    SwitchTo(PlayerState.Idle);
            }
        }
        else
        {
            // 空中：上升=Jump，下落=Fall
            if (_state != PlayerState.Fall)
            {
                // 从地面进入 Fall → 启动土狼时间计数器
                if (_state == PlayerState.Idle || _state == PlayerState.Run || _state == PlayerState.Crouch)
                    _coyoteTimeCounter = coyoteTime;

                SwitchTo(PlayerState.Fall);
            }
        }
    }

    // ============================================================
    //  保留字段（以兼容动画事件 / Inspector 绑定）
    //  新代码请使用 _state 和 SwitchTo()，不要直接修改这些字段！
    // ============================================================
    [HideInInspector] public bool isHurt;
    [HideInInspector] public bool isDead;
    [HideInInspector] public bool isAttack;
    [HideInInspector] public bool isCrouch;
    [HideInInspector] public bool isDialogue;
    [HideInInspector] public bool isJumping;
    [HideInInspector] public bool isSlide;
}
