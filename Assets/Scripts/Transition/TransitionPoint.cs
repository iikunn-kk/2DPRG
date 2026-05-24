using System;
using System.Collections;
using System.Collections.Generic;
// 移除不需要的 using 指令
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 传送点类，用于处理游戏中的场景传送逻辑。当玩家靠近传送点并按下指定按键时，触发场景传送。
/// </summary>
public class TransitionPoint : MonoBehaviour
{
    /// <summary>
    /// 传送类型枚举，定义了两种传送类型：同场景传送和不同场景传送。
    /// </summary>
    public enum TransitionType
    {
        SameScene, // 同一场景内的传送
        DifferentScene // 不同场景间的传送
    }

    [Header("Transition Info")]
    public string sceneName; // 目标场景的名称，当传送类型为不同场景传送时使用
    public TransitionType transitionType; // 当前传送点的传送类型
    public TransitionDestination.DestinationTag destinationTag; // 目标传送点的标记，用于定位目标位置
    public bool canTrans; // 标记玩家是否可以触发传送

    private PlayerInputController inputControl;

    // public SceneController sceneController;

    void Awake()
    {
        inputControl = new PlayerInputController();
        inputControl.Gameplay.Confirm.started += PortalTransform;
    }

    private void OnEnable()
    {
        inputControl.Enable();
    }

    private void OnDisable()
    {
        inputControl.Disable();
    }

    /// <summary>
    /// 当玩家碰撞体持续停留在传送点碰撞体内时调用。
    /// 若碰撞对象为玩家，则将可传送标记设置为 true。
    /// </summary>
    /// <param name="other">与传送点发生碰撞的碰撞体。</param>
    void OnTriggerStay2D(Collider2D other)
    {
        // 检查碰撞对象是否为玩家
        if (other.CompareTag("Player"))
        {
            // Debug.Log("进入传送状态");
            canTrans = true; // 设置可传送标记为 true
        }
    }

    /// <summary>
    /// 当玩家碰撞体离开传送点碰撞体时调用此方法。
    /// 若离开对象为玩家，则将可传送标记设置为 false。
    /// </summary>
    /// <param name="other">离开传送点碰撞体的碰撞体。</param>
    void OnTriggerExit2D(Collider2D other)
    {
        // 检查离开对象是否为玩家
        if (other.CompareTag("Player"))
        {
            // Debug.Log("退出传送状态");
            canTrans = false; // 设置可传送标记为 false
        }
    }

    private void PortalTransform(InputAction.CallbackContext context)
    {
        if (canTrans)
        {
            Debug.Log("传送");
            // 调用场景控制器的传送方法，传入当前传送点实例
            SceneController.Instance.TransitionToDestination(this);
        }
    }
}
