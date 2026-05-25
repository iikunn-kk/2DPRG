using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 传送点组件
/// 当玩家靠近并按下确认键时，触发场景传送
/// </summary>
public class TransitionPoint : MonoBehaviour
{
    public enum TransitionType
    {
        SameScene,
        DifferentScene
    }

    [Header("传送信息")]
    public string sceneName;
    public TransitionType transitionType;
    public TransitionDestination.DestinationTag destinationTag;

    [Header("状态")]
    public bool canTrans;

    private PlayerInputController _inputControl;

    private void Awake()
    {
        _inputControl = new PlayerInputController();
        _inputControl.Gameplay.Confirm.started += PortalTransform;
    }

    private void OnEnable()
    {
        _inputControl?.Enable();
    }

    private void OnDisable()
    {
        _inputControl?.Disable();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            canTrans = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            canTrans = false;
    }

    private void PortalTransform(InputAction.CallbackContext context)
    {
        if (!canTrans) return;

        if (SceneController.Instance == null)
        {
            Debug.LogError("[TransitionPoint] SceneController 实例不存在，无法传送");
            return;
        }

        SceneController.Instance.TransitionToDestination(this);
    }
}
