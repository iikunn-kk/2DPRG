using UnityEngine;

/// <summary>
/// 攻击结束的 Animator StateMachineBehaviour
/// OnStateExit: 重置玩家攻击状态
/// </summary>
public class AttackFinish : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var controller = animator.GetComponent<PlayerController>();
        if (controller != null)
            controller.isAttack = false;
    }
}
