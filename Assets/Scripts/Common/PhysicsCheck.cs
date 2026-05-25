using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 物理检测组件
/// 检测地面、墙壁、空气墙等碰撞状态
/// </summary>
public class PhysicsCheck : MonoBehaviour
{
    private CapsuleCollider2D coll;
    private PlayerController playerController;
    
    [Header("检测参数")]
    public bool manual;
    public bool isPlayer;
    public Vector2 bottomOffset;
    public Vector2 leftOffset;
    public Vector2 rightOffset;
    
    [Header("检测结果")]
    public bool isGround;
    public bool touchLeftWall;
    public bool touchRightWall;
    public bool touchLeftAirWall;
    public bool touchRightAirWall;
    public bool onWall;
    
    [Header("层级设置")]
    public LayerMask groundLayer;
    public LayerMask airWallLayer;
    
    [Header("检测半径")]
    [SerializeField] private float checkRadius = 0.15f; // 提取魔法数字

    private void Awake()
    {
        coll = GetComponent<CapsuleCollider2D>();
        if (!manual)
        {
            rightOffset = new Vector2((coll.bounds.size.x + coll.offset.x) / 2, coll.bounds.size.y / 2);
            leftOffset = new Vector2(-rightOffset.x, rightOffset.y);
            bottomOffset = new Vector2(0, coll.offset.y - coll.size.y / 2);
        }
        if (isPlayer)
            playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        Check();
    }

    /// <summary>
    /// 执行所有物理检测
    /// </summary>
    public void Check()
    {
        // 检测地面
        isGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, checkRadius, groundLayer);
        
        // 通知事件中心：地面状态变更（使用常量名称）
        EventCenter.Instance.Trigger(EventCenter.Events.IsGround, isGround);
        
        // 检测左边实体墙
        touchLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, checkRadius, groundLayer);
        
        // 检测右边实体墙
        touchRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, checkRadius, groundLayer);
        
        // 检测左边空气墙
        touchLeftAirWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, checkRadius, airWallLayer);
        
        // 检测右边空气墙
        touchRightAirWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, checkRadius, airWallLayer);
        
        if (isPlayer)
            onWall = ((touchLeftWall && playerController.inputDirection.x < 0f) || (touchRightWall && playerController.inputDirection.x > 0f)) && !isGround;
    }

    /// <summary>
    /// 在 Scene 视图中绘制检测球体 Gizmos
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, checkRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, checkRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, checkRadius);
    }
}
