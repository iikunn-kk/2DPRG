using UnityEngine;

/// <summary>
/// 野猪死亡音效触发器（Animator StateMachineBehaviour）
/// 当进入死亡动画状态时自动播放死亡音效
/// </summary>
public class BoarDeadAudio : StateMachineBehaviour
{
    /// <summary>
    /// 进入死亡状态时播放音效
    /// </summary>
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 使用统一的 EnemiesAudio 组件（替代已删除的 BoarAudio）
        EnemiesAudio enemiesAudio = animator.GetComponent<EnemiesAudio>();
        
        if (enemiesAudio != null)
        {
            enemiesAudio.PlayDeadSound();
        }
        else
        {
            Debug.LogWarning("[BoarDeadAudio] 未找到 EnemiesAudio 组件");
        }
    }
}
