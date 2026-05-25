using UnityEngine;

/// <summary>
/// 攻击1碰撞体偏移控制 (StateMachineBehaviour)
/// OnStateUpdate: 持续调整碰撞体位置实现攻击范围偏移
/// </summary>
public class AttackChangeCollider : StateMachineBehaviour
{
    private CapsuleCollider2D _cachedCollider;
    private const float OffsetX = -0.95f;

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_cachedCollider == null)
            _cachedCollider = animator.GetComponent<CapsuleCollider2D>();

        if (_cachedCollider != null)
            _cachedCollider.offset = new Vector2(OffsetX, _cachedCollider.offset.y);
    }
}
