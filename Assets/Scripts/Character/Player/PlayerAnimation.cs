using System.Collections;
using UnityEngine;

/// <summary>
/// 玩家动画控制器
/// 负责根据玩家状态同步更新 Animator 参数
/// </summary>
public class PlayerAnimation : Singleton<PlayerAnimation>
{
    private Animator anim;
    private Rigidbody2D rb;
    private PhysicsCheck physicsCheck;
    private PlayerController playerController;

    protected override void Awake()
    {
        base.Awake();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
        playerController = GetComponent<PlayerController>();

        // 验证组件引用
        if (anim == null) Debug.LogWarning("[PlayerAnimation] Animator 组件未找到");
        if (rb == null) Debug.LogWarning("[PlayerAnimation] Rigidbody2D 组件未找到");
        if (physicsCheck == null) Debug.LogWarning("[PlayerAnimation] PhysicsCheck 组件未找到");
        if (playerController == null) Debug.LogWarning("[PlayerAnimation] PlayerController 组件未找到");
    }

    /// <summary>
    /// 每帧驱动动画参数更新
    /// ⚠️ 这是动画系统的核心循环，不可删除！
    /// </summary>
    private void Update()
    {
        SetAnimation();
    }

    /// <summary>
    /// 每帧更新动画参数
    /// </summary>
    public void SetAnimation()
    {
        // 安全检查：所有组件必须有效
        if (anim == null || rb == null || physicsCheck == null || playerController == null)
            return;

        // 更新移动相关参数
        anim.SetFloat("velocityX", Mathf.Abs(rb.velocity.x)); // 取绝对值，因为跑步条件是 velocityX > 0.1
        anim.SetFloat("velocityY", rb.velocity.y);
        
        // 更新状态参数
        anim.SetBool("isGround", physicsCheck.isGround);
        anim.SetBool("isCrouch", playerController.isCrouch);
        anim.SetBool("isDead", playerController.isDead);
        anim.SetBool("isAttack", playerController.isAttack);
        anim.SetBool("onWall", physicsCheck.onWall);
        anim.SetBool("isSlide", playerController.isSlide);
    }

    /// <summary>
    /// 播放受伤动画
    /// </summary>
    public void PlayHurt()
    {
        if (anim != null)
            anim.SetTrigger("hurt");
    }

    /// <summary>
    /// 播放攻击动画
    /// </summary>
    public void PlayAttack()
    {
        if (anim != null)
            anim.SetTrigger("attack");
    }
}
