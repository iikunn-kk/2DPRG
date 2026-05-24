using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// 玩家控制器。
/// 使用 PlayerState 枚举 + SwitchTo() 保证状态互斥，取代原来的 7 个无约束 bool。
/// </summary>
public class PlayerController : Singleton<PlayerController>
{
    [Header("土狼时间参数")]
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float coyoteTime = 0.1f;

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

    // 事件委托字段：必须存为字段，确保 Add/Remove 使用同一个引用，防止场景切换时事件泄漏
    private UnityAction<bool> _onGroundChanged;

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
        _onGroundChanged = CoyoteTime =>
        {
            if (CoyoteTime == false)
            {
                if (rb == null) return; // 场景切换时 rb 可能已被销毁
                rb.gravityScale = 0;
                StartCoroutine(WaitForCoyoteTime());
            }
        };
        EventCenter.Instance.AddEventListener<bool>("isGround", _onGroundChanged);
        inputControl.Enable();
        GameManager.Instance.RigisterPlayer(characterStats);
        Debug.Log("注册成功");
    }

    void Start()
    {
        SaveManager.Instance.LoadPlayerData();
        Debug.Log("人物数据加完毕");
    }

    private void OnDisable()
    {
        if (_onGroundChanged != null)
        {
            EventCenter.Instance.RemoveEventListener<bool>("isGround", _onGroundChanged);
            _onGroundChanged = null;
        }
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

        // 土狼时间：落地且不在跳跃状态时重置二段跳
        if ((physicsCheck.isGround || physicsCheck.onWall) && _state != PlayerState.Jump)
        {
            currentNumberJumps = 0;
        }

        CheckState();
        UpdateMovementState();
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
        // Dead 是终态，不可被任何状态覆盖（除了重启）
        if (_state == PlayerState.Dead && newState != PlayerState.Dead)
            return;

        // Hurt 可以被 Dead 打断、可以恢复（Idle/Fall/Run）、允许重新受击，
        // 但禁止 Attack/Jump/Slide 在受伤期间激活
        if (_state == PlayerState.Hurt &&
            (newState == PlayerState.Attack ||
             newState == PlayerState.Jump ||
             newState == PlayerState.Slide))
            return;

        ExitState(_state);
        _state = newState;
        EnterState(_state);
    }

    private void EnterState(PlayerState state)
    {
        // 关键：同步 bool 字段 → Animator 通过 PlayerAnimation.SetAnimation() 读取这些字段
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

    private void ExitState(PlayerState state)
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
        }
    }

    // ============================================================
    //  移动
    // ============================================================

    public void Move()
    {
        rb.velocity = new Vector2(inputDirection.x * currentSpeed, rb.velocity.y);
        transform.localScale = new Vector3(tempX * faceDir, transform.localScale.y, transform.localScale.z);

        // 下蹲
        bool shouldCrouch = inputDirection.y < -0.5f && physicsCheck.isGround;
        if (shouldCrouch && _state != PlayerState.Slide)
        {
            SwitchTo(PlayerState.Crouch);
            coll.offset = new Vector2(-0.18f, 0.8303528f);
            coll.size = new Vector2(1.3125f, 1.761332f);
        }
        else if (!shouldCrouch && _state == PlayerState.Crouch)
        {
            SwitchTo(PlayerState.Run);
            coll.size = originalSize;
            coll.offset = originalOffset;
        }
    }

    // ============================================================
    //  输入回调
    // ============================================================

    private void Jump(InputAction.CallbackContext context)
    {
        // Dialogue 和 Dead 状态禁止输入
        if (_state == PlayerState.Dead || _state == PlayerState.Dialogue)
            return;

        if (currentNumberJumps < maximumNumberJumps)
        {
            // 如果正在滑铲，先终止
            if (_state == PlayerState.Slide)
            {
                SwitchTo(PlayerState.Jump);
                StopCoroutine("TriggerSlide");
            }

            currentNumberJumps++;
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            SwitchTo(PlayerState.Jump);

            if (_jumpResetCoroutine != null) StopCoroutine(_jumpResetCoroutine);
            _jumpResetCoroutine = StartCoroutine(ResetJumpingState());
        }
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

    private IEnumerator WaitForCoyoteTime()
    {
        yield return new WaitForSeconds(coyoteTime);
        rb.gravityScale = 4;
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
                SwitchTo(PlayerState.Fall);
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
