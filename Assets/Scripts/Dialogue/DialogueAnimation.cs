using UnityEngine;

/// <summary>
/// 对话动画控制
/// 通过 Animator 播放对话攻击动画
/// </summary>
public class DialogueAnimation : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// 播放对话时的攻击动画
    /// </summary>
    public void PlayDialogueWithAttack2()
    {
        if (_animator != null)
            _animator.Play("Attack2");
    }
}
