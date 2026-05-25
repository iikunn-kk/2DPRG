using UnityEngine;

/// <summary>
/// 死亡音效的 Animator StateMachineBehaviour
/// OnStateEnter: 播放角色死亡音效
/// </summary>
public class DeathAudio : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var playerAudio = animator.GetComponent<PlayerAudio>();
        if (playerAudio?.audioSource1 != null)
        {
            playerAudio.audioSource1.clip = playerAudio.dead;
            playerAudio.audioSource1.Play();
        }
    }
}
