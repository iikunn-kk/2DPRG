using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunAudio : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerAudio playerAudio = animator.GetComponent<PlayerAudio>();
        if (playerAudio != null)
        {
            string mapType = GetCurrentMapType(animator.gameObject);
            if (!playerAudio.audioSource2.isPlaying)
            {
                playerAudio.PlayRunningSound(mapType);
                // playerAudio.audioSource2.clip = playerAudio.running;
                // playerAudio.audioSource2.Play();
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerAudio playerAudio = animator.GetComponent<PlayerAudio>();
        playerAudio.audioSource2.Stop();
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
    // 使用射线自动检测地面是什么标签
    private string GetCurrentMapType(GameObject player)
    {
        int layerMask = LayerMask.GetMask("Ground"); // 假设地面物体在 "Ground" 层级
        RaycastHit2D hit = Physics2D.Raycast(player.transform.position, Vector2.down, Mathf.Infinity, layerMask);
        if (hit.collider != null)
        {
            // Debug.Log("二维射线击中的物体是：" + hit.collider.tag);
            return hit.collider.tag;
        }
        else
        {
            Debug.Log("二维射线检测未在“地面”层上击中任何二维碰撞体。");
        }
        return "";
    }
}
