using UnityEngine;

/// <summary>
/// 跑步音效的 Animator StateMachineBehaviour
/// OnStateUpdate: 持续播放跑步音效
/// OnStateExit: 停止跑步音效
/// </summary>
public class RunAudio : StateMachineBehaviour
{
    private PlayerAudio _cachedPlayerAudio;

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_cachedPlayerAudio == null)
            _cachedPlayerAudio = animator.GetComponent<PlayerAudio>();

        if (_cachedPlayerAudio?.audioSource2 != null && !_cachedPlayerAudio.audioSource2.isPlaying)
        {
            _cachedPlayerAudio.PlayRunningSound();
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_cachedPlayerAudio == null)
            _cachedPlayerAudio = animator.GetComponent<PlayerAudio>();

        if (_cachedPlayerAudio?.audioSource2 != null)
            _cachedPlayerAudio.audioSource2.Stop();
    }
}
