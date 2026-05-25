using UnityEngine;

/// <summary>
/// 野猪受伤音效触发器（Animator StateMachineBehaviour）
/// 当进入受伤动画状态时自动播放受伤音效
/// </summary>
public class BoarHurtAudio : StateMachineBehaviour
{
    /// <summary>
    /// 进入受伤状态时播放音效
    /// </summary>
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 使用统一的 EnemiesAudio 组件（替代已删除的 BoarAudio）
        EnemiesAudio enemiesAudio = animator.GetComponent<EnemiesAudio>();
        
        if (enemiesAudio != null)
        {
            enemiesAudio.PlayHurtSound();
        }
        else
        {
            Debug.LogWarning("[BoarHurtAudio] 未找到 EnemiesAudio 组件");
        }
    }
}
