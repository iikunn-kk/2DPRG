using UnityEngine;

/// <summary>
/// 受伤动画 StateMachineBehaviour
/// OnStateExit: 重置玩家受伤状态
/// </summary>
public class HurtAnimation : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var controller = animator.GetComponent<PlayerController>();
        if (controller != null)
            controller.isHurt = false;
    }
}
