using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 交互提示标识
/// 管理交互提示图标的显示/隐藏与玩家确认输入
/// </summary>
public class Sign : MonoBehaviour
{
    private PlayerInputController _playerInput;
    private Animator _anim;
    private SpriteRenderer _sr;
    private IInteractable _targetItem;

    [Header("引用")]
    public Transform playerTrans;
    public GameObject signSprite;

    [Header("状态")]
    public bool canPress;

    private void Awake()
    {
        if (signSprite != null)
        {
            _anim = signSprite.GetComponent<Animator>();
            _sr = signSprite.GetComponent<SpriteRenderer>();
        }

        _playerInput = new PlayerInputController();
        _playerInput.Enable();
    }

    private void OnEnable()
    {
        _playerInput.Gameplay.Confirm.started += OnConfirm;
    }

    private void OnDisable()
    {
        _playerInput.Gameplay.Confirm.started -= OnConfirm;
    }

    private void Update()
    {
        if (_sr != null)
            _sr.enabled = canPress;

        if (signSprite != null && playerTrans != null)
            signSprite.transform.localScale = playerTrans.localScale;
    }

    private void OnConfirm(InputAction.CallbackContext context)
    {
        if (canPress && _targetItem != null)
            _targetItem.TriggerAction();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
        {
            canPress = true;
            _targetItem = other.GetComponent<IInteractable>();
        }
        else
        {
            canPress = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        canPress = false;
    }
}
