using UnityEngine;

/// <summary>
/// 玩家受伤音效触发器（Animator StateMachineBehaviour）
/// 当进入受伤动画状态时自动播放受伤音效
/// </summary>
public class HurtAudio : StateMachineBehaviour
{
    /// <summary>
    /// 进入受伤状态时播放音效
    /// </summary>
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerAudio playerAudio = animator.GetComponent<PlayerAudio>();
        
        if (playerAudio != null)
        {
            playerAudio.PlayHurtSound(); // 使用新的统一方法
        }
        else
        {
            Debug.LogWarning("[HurtAudio] 未找到 PlayerAudio 组件");
        }
    }
}
