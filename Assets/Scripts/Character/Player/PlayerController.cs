using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : Singleton<PlayerController>
{
    [Header("土狼时间参数")]
    [SerializeField] float runSpeed = 5f; // 土狼时间状态下的移动速度
    [SerializeField] float coyoteTime = 0.1f; // 土狼时间持续时间(秒)


    [Header("二段跳计数器")]
    public float maximumNumberJumps; // 最大跳跃次数
    public float currentNumberJumps; // 当前跳跃次数

    private CharacterStats characterStats;

    [Header("基本参数")]
    public float hurtForce;// 受击时受到的力
    public float currentSpeed; // 水平移动速度
    public float jumpForce;// 跳跃力度
    public float slideDistance;//滑铲距离
    public float slideSpeed;//滑铲速度
    public int slidePowerCost;//滑铲能量消耗数值
    public float tempX;
    public float faceDir;
    private float slideStartPos; // 滑铲开始时的X坐标
    private float currentSlideDirection; // 当前滑铲方向

    private Vector2 originalOffset;// 原始碰撞体偏移量
    private Vector2 originalSize;// 原始碰撞体尺寸

    [Header("物理材质")]
    public PhysicsMaterial2D normal;// 普通地面物理材质
    public PhysicsMaterial2D wall;// 墙面物理材质

    private PlayerInputController inputControl;// 输入控制系统
    private Rigidbody2D rb;// 刚体组件
    private CapsuleCollider2D coll;// 碰撞体组件
    private SpriteRenderer sr;// 精灵渲染器
    private PlayerAnimation playerAnimation; // 动画控制器
    private PhysicsCheck physicsCheck;// 物理检测组件
    private Character character;
    public Vector2 inputDirection;

    [Header("Bool状态")]
    public bool isHurt; // 是否处于受伤状态
    public bool isDead;// 是否死亡
    public bool isAttack;// 是否正在攻击
    public bool isCrouch;// 是否处于下蹲状态
    public bool isDialogue;// 是否处于对话状态

    public bool isJumping; // 跳跃状态标记
    public bool isSlide;//是否处于滑铲

    private PlayerAudio playerAudio;// 音频控制器
    public UnityEvent PlayRunAudio;// 移动音效事件
    public UnityEvent StopRunAudio;// 停止移动音效事件

    protected override void Awake()
    {
        base.Awake();

        //组件引用
        inputControl = new PlayerInputController();

        characterStats = GetComponent<CharacterStats>();

        rb = GetComponent<Rigidbody2D>();

        sr = GetComponent<SpriteRenderer>();

        physicsCheck = GetComponent<PhysicsCheck>();

        coll = GetComponent<CapsuleCollider2D>();

        playerAnimation = GetComponent<PlayerAnimation>();

        playerAudio = GetComponent<PlayerAudio>();
        character = GetComponent<Character>();

        //事件注册
        inputControl.Gameplay.Jump.started += Jump;//跳跃

        inputControl.Gameplay.Attack.started += PlayerAttack;//攻击

        inputControl.Gameplay.Slide.started += Slide;//滑铲

        //参数赋值
        originalOffset = coll.offset;

        originalSize = coll.size;

        tempX = transform.localScale.x;

        faceDir = 1;

        currentNumberJumps = 0;
    }



    private void OnEnable()
    {
        EventCenter.Instance.AddEventListener<bool>("isGround", CoyoteTime =>
        {
            if (CoyoteTime == false)
            {
                rb.gravityScale = 0;
                StartCoroutine(WaitForCoyoteTime());
            }
        });
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
        EventCenter.Instance.RemoveEventListener<bool>("isGround", CoyoteTime =>
        {
            if (CoyoteTime == false)
            {
                rb.gravityScale = 0;
            }
        });
        if (inputControl != null)
        {
            inputControl.Disable();
        }
    }

    private void Update()
    {
        inputDirection = inputControl.Gameplay.Move.ReadValue<Vector2>();
        CheckState();

        // 只有当角色不在跳跃状态时，才重置跳跃计数器
        if ((physicsCheck.isGround || physicsCheck.onWall) && !isJumping)
        {
            currentNumberJumps = 0;
        }
    }

    private void FixedUpdate()
    {
        if (!isHurt && !isDialogue)
        {
            Move();
        }
    }

    public void Move()
    {
        rb.velocity = new Vector2(inputDirection.x * currentSpeed, rb.velocity.y);

        // // 根据输入方向翻转角色朝向
        // if (inputDirection.x < 0)
        // {
        //     sr.flipX = true;
        // }
        // if (inputDirection.x > 0)
        // {
        //     sr.flipX = false;
        // }
        //根据输入方向翻转角色朝向
        // int faceDir = (int)transform.localScale.x;


        if (inputDirection.x > 0)
        {
            faceDir = 1;  // 面朝右
        }
        if (inputDirection.x < 0)
        {
            faceDir = -1; // 面朝左
        }
        // Debug.Log(faceDir);
        // Debug.Log(tempX);
        transform.localScale = new Vector3(tempX * faceDir, transform.localScale.y, transform.localScale.z);


        //transform.localScale = new Vector3(faceDir, 1.5f, 1.5f);
        // Vector3 tempScale = transform.localScale;
        // float tempx = tempScale.x;
        // tempScale.x = faceDir * tempScale.x;
        // transform.localScale = tempScale;


        // 下蹲
        isCrouch = inputDirection.y < -0.5f && physicsCheck.isGround;
        if (isCrouch)
        {
            // 修改碰撞体大小和位移
            coll.offset = new Vector2(-0.18f, 0.8303528f);
            coll.size = new Vector2(1.3125f, 1.761332f);
        }
        else
        {
            // 还原之前碰撞体的体积
            coll.size = originalSize;
            coll.offset = originalOffset;
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (isDialogue)
            return;

        //Debug.Log("跳跃了一次");
        // 检查是否可以跳跃
        if (currentNumberJumps < maximumNumberJumps)
        {


            isSlide = false;
            StopCoroutine("TriggerSlide");


            currentNumberJumps++;
            //Debug.Log("跳跃次数加一");
            // 执行跳跃动作
            // rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);


            playerAudio.PlayWithJump();
            isJumping = true;
            // 在一定时间后重置跳跃状态
            StartCoroutine(ResetJumpingState());
        }
    }

    private void PlayerAttack(InputAction.CallbackContext context)
    {
        if (isDialogue)
            return;

        playerAnimation.PlayAttack();
        isAttack = true;
    }

    //滑铲的源代码

    // private void Slide(InputAction.CallbackContext context)
    // {
    //     if (isDialogue)
    //         return;

    //     if (!isSlide && physicsCheck.isGround && character.currentPower >= slidePowerCost)
    //     {
    //         isSlide = true;
    //         var targetPos = new Vector3(transform.position.x + slideDistance * transform.localScale.x, transform.position.y);//滑铲目的地、
    //         Debug.Log("滑铲的终点为" + targetPos);
    //         gameObject.layer = LayerMask.NameToLayer("Enemy");//修改layer实现滑铲过程过玩家碰到敌人不扣血功能
    //         StartCoroutine(TriggerSlide(targetPos));

    //         character.OnSlide(slidePowerCost);
    //     }
    // }
    // private IEnumerator TriggerSlide(Vector3 target)
    // {
    //     do
    //     {
    //         yield return null;
    //         if (!physicsCheck.isGround)
    //             break;
    //         if (physicsCheck.touchLeftWall && transform.localScale.x < 0f || physicsCheck.touchRightWall && transform.localScale.x > 0f || physicsCheck.touchLeftAirWall && transform.localScale.x < 0f || physicsCheck.touchRightAirWall && transform.localScale.x > 0f)//滑铲过程中撞墙
    //         {
    //             isSlide = false;
    //             break;
    //         }

    //         rb.MovePosition(new Vector2(transform.position.x + transform.localScale.x * slideSpeed, transform.position.y));//使用MovePositionAPI实现滑铲的移动逻辑
    //         Debug.Log("玩家当前的位置为" + transform.position.x);
    //         Debug.Log("滑铲的始终点间距为" + Mathf.Abs(target.x - transform.position.x));
    //         //target.x = -10,  
    //     } while (Mathf.Abs(target.x - transform.position.x) > 0.1f);//如果滑铲目的地与现在的坐标距离大于0.1f，继续滑铲
    //     // || (Mathf.Abs(target.x + slideDistance - transform.position.x) > 0.1f)
    //     isSlide = false;
    //     gameObject.layer = LayerMask.NameToLayer("Player");//滑铲结束修改Layer为Player
    // }





    //第一次改进滑铲逻辑后的代码
    private void Slide(InputAction.CallbackContext context)
    {
        if (isDialogue)// 对话状态下不能滑铲
            return;
        // 滑铲启动的核心条件：
        // 1. 不在滑铲状态（!isSlide）
        // 2. 处于地面（physicsCheck.isGround，通过物理检测组件判断）
        // 3. 能量足够（character.currentPower >= 滑铲能量消耗）
        if (!isSlide && physicsCheck.isGround && character.currentPower >= slidePowerCost)
        {
            isSlide = true;// 标记为滑铲状态
            // 记录滑铲开始时的位置和方向
            slideStartPos = transform.position.x; // 记录滑铲开始的X坐标
            Debug.Log("滑铲的起始点为" + slideStartPos);
            currentSlideDirection = transform.localScale.x; // 1为右，-1为左

            // 滑铲时临时将玩家层改为"Enemy"，用于避免滑铲过程中与敌人碰撞受伤
            gameObject.layer = LayerMask.NameToLayer("Enemy");
            StartCoroutine(TriggerSlide()); // 启动滑铲移动的协程

            character.OnSlide(slidePowerCost);// 消耗滑铲所需的能量
        }
    }

    private IEnumerator TriggerSlide()
    {
        do
        {
            yield return null;// 等待一帧，避免阻塞帧更新
            // 终止条件1：滑铲过程中离开地面（不在地面则中断滑铲）
            if (!physicsCheck.isGround)
                break;

            // 处理滑铲过程中方向改变的情况：
            // 若玩家在滑铲时转向（左右翻转），则更新滑铲方向和起始位置
            if (transform.localScale.x != currentSlideDirection)
            {
                // 更新滑铲方向和起始位置
                currentSlideDirection = transform.localScale.x;// 更新方向
                slideStartPos = transform.position.x;// 重置起始位置（以当前位置为新起点）
            }

            // 计算当前滑铲的目标终点X坐标（起始位置 + 方向×总距离）
            float targetX = slideStartPos + currentSlideDirection * slideDistance;
            // 终止条件2：滑铲过程中撞墙（左实体墙且向左滑、或右实体墙且向右滑、左空气墙且向左滑、或右空气墙且向右滑）
            if (physicsCheck.touchLeftWall && currentSlideDirection < 0f ||
                physicsCheck.touchRightWall && currentSlideDirection > 0f ||
                physicsCheck.touchLeftAirWall && currentSlideDirection < 0f ||
                physicsCheck.touchRightAirWall && currentSlideDirection > 0f)
            {
                isSlide = false;
                break;
            }
            // 执行滑铲移动：根据方向和速度更新位置（Y坐标不变，保持在地面）
            rb.MovePosition(new Vector2(transform.position.x + currentSlideDirection * slideSpeed, transform.position.y));

            Debug.Log("玩家当前的位置为" + transform.position.x);
            Debug.Log("滑铲的目标终点为" + targetX);
            Debug.Log("滑铲的始终点间距为" + Mathf.Abs(targetX - transform.position.x));

            // 终止条件3：到达滑铲总距离（当前位置与目标终点的差距小于0.1f时结束）
        } while (Mathf.Abs(slideStartPos + currentSlideDirection * slideDistance - transform.position.x) > 0.9f);
        // 滑铲结束：重置状态
        isSlide = false;
        gameObject.layer = LayerMask.NameToLayer("Player");// 恢复玩家层
    }

    // 第二次改进滑铲逻辑后的代码
    // 新增：用于记录滑铲过程中已滑动的距离（核心新增变量）
    // private float slideDistanceCovered;

    // private void Slide(InputAction.CallbackContext context)
    // {
    //     if (isDialogue)
    //         return;

    //     // 滑铲启动条件不变
    //     if (!isSlide && physicsCheck.isGround && character.currentPower >= slidePowerCost)
    //     {
    //         isSlide = true;
    //         slideStartPos = transform.position.x; // 记录滑铲起始X坐标
    //         currentSlideDirection = transform.localScale.x; // 1=右，-1=左
    //         // 新增：初始化已滑动距离为0（每次滑铲重新计数）
    //         slideDistanceCovered = 0f;

    //         gameObject.layer = LayerMask.NameToLayer("Enemy");
    //         StartCoroutine(TriggerSlide());
    //         character.OnSlide(slidePowerCost);
    //     }
    // }

    // private IEnumerator TriggerSlide()
    // {
    //     do
    //     {
    //         yield return null; // 等待一帧，避免阻塞帧更新

    //         // 边界条件1：滑铲过程中离开地面，立即终止
    //         if (!physicsCheck.isGround)
    //             break;

    //         // 边界条件2：滑铲过程中撞墙，立即终止
    //         if ((physicsCheck.touchLeftWall && currentSlideDirection < 0f) ||
    //             (physicsCheck.touchRightWall && currentSlideDirection > 0f) ||
    //             (physicsCheck.touchLeftAirWall && currentSlideDirection < 0f) ||
    //             (physicsCheck.touchRightAirWall && currentSlideDirection > 0f))
    //         {
    //             break;
    //         }

    //         //  核心改进1：处理方向切换
    //         // 若滑铲中转向，重置“起始位置”和“已滑动距离”（以当前位置为新起点）
    //         if (transform.localScale.x != currentSlideDirection)
    //         {
    //             currentSlideDirection = transform.localScale.x;
    //             slideStartPos = transform.position.x;
    //             slideDistanceCovered = 0f; // 方向变了，重新计算已滑距离
    //         }

    //         // 核心改进2：计算本次移动距离 
    //         // 1. 记录移动前的位置（用于计算本帧实际滑动距离）
    //         float posBeforeMove = transform.position.x;
    //         // 2. 执行滑铲移动（Y坐标不变，保持地面）
    //         rb.MovePosition(new Vector2(transform.position.x + currentSlideDirection * slideSpeed, transform.position.y));
    //         // 3. 计算本帧实际滑动的距离（取绝对值，避免方向影响）
    //         float distanceThisFrame = Mathf.Abs(transform.position.x - posBeforeMove);
    //         // 4. 累计已滑动的总距离（不超过最大滑铲距离，避免超量）
    //         slideDistanceCovered = Mathf.Min(slideDistanceCovered + distanceThisFrame, slideDistance);

    //         // 调试日志（可选保留，便于验证） 
    //         float currentTargetX = slideStartPos + currentSlideDirection * slideDistance; // 当前终点
    //         Debug.Log($"当前位置：{transform.position.x:F2} | 终点：{currentTargetX:F2} | 已滑距离：{slideDistanceCovered:F2}/{slideDistance:F2}");

    //         //  核心改进3：新终止条件 
    //         // 当“已滑距离 ≥ 总滑铲距离”时，终止滑铲（忽略0.1f误差，直接判断是否滑够距离）
    //     } while (slideDistanceCovered < slideDistance);

    //     // 滑铲结束：重置状态
    //     isSlide = false;
    //     gameObject.layer = LayerMask.NameToLayer("Player");
    //     slideDistanceCovered = 0f; // 重置已滑距离，避免下次滑铲受影响
    // }
    private IEnumerator ResetJumpingState()
    {
        yield return new WaitForSeconds(0.1f); // 延迟0.1秒
        isJumping = false;
    }

    private IEnumerator WaitForCoyoteTime()
    {
        yield return new WaitForSeconds(coyoteTime);
        rb.gravityScale = 4;
    }
    void OnTriggerStay2D(Collider2D other)
    {
        // Debug.Log(other.name);
    }

    public void GetHurt(Transform attacker)
    {
        isHurt = true;
        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2(transform.position.x - attacker.position.x, 0).normalized;
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
    }

    public void PlayerDead()
    {
        if (isDead) return; // 已死亡则不再执行
        isDead = true;
        inputControl.Gameplay.Disable();
        PausePanel.Instance.DeadToRestartGame();//先播放死亡动画，延迟两秒打开暂停面板
    }

    public void OndialogueStopMove()
    {
        isDialogue = true;
    }

    public void noOnDialogueRecoverMove()
    {
        isDialogue = false;
    }

    private void CheckState()
    {
        coll.sharedMaterial = physicsCheck.isGround ? normal : wall;
        if (physicsCheck.onWall)
            // rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2f);
            rb.velocity = new Vector2(rb.velocity.x, 0f);
        else
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
    }
}