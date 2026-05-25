using UnityEngine;

/// <summary>
/// 自我销毁 StateMachineBehaviour
/// OnStateExit: 销毁自身组件（用于清理临时动画状态）
/// </summary>
public class DestroyMySelf : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Destroy(animator.gameObject);
    }
}
