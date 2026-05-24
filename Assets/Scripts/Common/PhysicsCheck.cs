using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    private CapsuleCollider2D coll;
    private PlayerController playerController;
    private Rigidbody2D rb;
    [Header("检测参数")]
    public bool manual;
    public bool isPlayer;
    public Vector2 bottomOffset;
    public Vector2 leftOffset;
    public Vector2 rightOffset;
    [Header("布尔值")]
    public bool isGround;
    public bool touchLeftWall;
    public bool touchRightWall;
    public bool touchLeftAirWall;
    public bool touchRightAirWall;
    public bool onWall;
    public LayerMask groundLayer;
    public LayerMask airWallLayer;
    public float checkRaduis;
    private void Awake()
    {


        coll = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        if (!manual)
        {
            rightOffset = new Vector2((coll.bounds.size.x + coll.offset.x) / 2, coll.bounds.size.y / 2);
            leftOffset = new Vector2(-rightOffset.x, rightOffset.y);
        }
        if (isPlayer)
            playerController = GetComponent<PlayerController>();
    }
    private void Update()
    {
        Check();
    }

    public void Check()
    {
        //检测地面
        isGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, checkRaduis, groundLayer);
        EventCenter.Instance.EventTrigger<bool>("isGround", false);
        //  检测左边实体墙
        touchLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, checkRaduis, groundLayer);
        //  检测右边实体墙
        touchRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, checkRaduis, groundLayer);
        //检测左边空气墙
        touchLeftAirWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, checkRaduis, airWallLayer);
        //检测右边空气墙
        touchRightAirWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, checkRaduis, airWallLayer);
        if (isPlayer)
            onWall = ((touchLeftWall && playerController.inputDirection.x < 0f) || (touchRightWall && playerController.inputDirection.x > 0f)) && !isGround;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, checkRaduis);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, checkRaduis);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, checkRaduis);
    }
}
