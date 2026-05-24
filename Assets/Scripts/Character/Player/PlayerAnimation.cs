using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAnimation : Singleton<PlayerAnimation>
{
    private Animator anim;
    private Rigidbody2D rb;
    private PhysicsCheck physicsCheck;
    private PlayerController playerController;

    // private  void Awake()
    // {
    //     anim = GetComponent<Animator>();
    //     rb = GetComponent<Rigidbody2D>();
    //     physicsCheck = GetComponent<PhysicsCheck>();
    //     playerController = GetComponent<PlayerController>();
    // }
    protected override void Awake()
    {
        base.Awake();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        SetAnimation();
    }

    public void SetAnimation()
    {
        anim.SetFloat("velocityX", Mathf.Abs(rb.velocity.x));//这里的x速度可以为负数，也就是向左跑的情况，这时候要取绝对值，因为跑步动画切换的条件是velocityX > 0.1
        anim.SetFloat("velocityY", rb.velocity.y);
        anim.SetBool("isGround", physicsCheck.isGround);
        anim.SetBool("isCrouch", playerController.isCrouch);
        anim.SetBool("isDead", playerController.isDead);
        anim.SetBool("isAttack", playerController.isAttack);
        anim.SetBool("onWall", physicsCheck.onWall);
        anim.SetBool("isSlide", playerController.isSlide);
    }
    public void PlayHurt()
    {
        anim.SetTrigger("hurt");
    }

    public void PlayAttack()
    {
        anim.SetTrigger("attack");
    }
}

