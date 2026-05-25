using UnityEngine;

/// <summary>
/// 攻击音效的 Animator StateMachineBehaviour
/// OnStateEnter: 播放攻击音效
/// OnStateUpdate: 标记攻击状态
/// OnStateExit: 停止攻击音效
/// </summary>
public class AttackAudio1 : StateMachineBehaviour
{
    private PlayerAudio _cachedPlayerAudio;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_cachedPlayerAudio == null)
            _cachedPlayerAudio = animator.GetComponent<PlayerAudio>();

        if (_cachedPlayerAudio?.audioSource1 != null)
        {
            _cachedPlayerAudio.audioSource1.clip = _cachedPlayerAudio.attacking;
            _cachedPlayerAudio.audioSource1.Play();
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_cachedPlayerAudio != null)
            _cachedPlayerAudio.attackAudio = true;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_cachedPlayerAudio?.audioSource1 != null && _cachedPlayerAudio.audioSource1.isPlaying)
            _cachedPlayerAudio.audioSource1.Stop();
    }
}
