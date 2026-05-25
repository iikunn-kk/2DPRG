using UnityEngine;

/// <summary>
/// 攻击2碰撞体偏移控制 (StateMachineBehaviour)
/// OnStateUpdate: 持续调整碰撞体位置实现攻击范围偏移
/// </summary>
public class Attack2ChangeCollider : StateMachineBehaviour
{
    private CapsuleCollider2D _cachedCollider;
    private const float OffsetX = -0.5f;

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_cachedCollider == null)
            _cachedCollider = animator.GetComponent<CapsuleCollider2D>();

        if (_cachedCollider != null)
            _cachedCollider.offset = new Vector2(OffsetX, _cachedCollider.offset.y);
    }
}
