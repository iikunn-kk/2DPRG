using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sign : MonoBehaviour
{
    private PlayerInputController playerInput;
    private Animator anim;                   // 提示动画控制器
    public Transform playerTrans;
    public GameObject signSprite;             // 提示精灵对象
    private IInteractable targetItem;
    public bool canPress;                    // 是否可以按下交互键的标志
    private SpriteRenderer sr;

    private void Awake()
    {
        // 初始化提示动画组件和输入系统
        anim = signSprite.GetComponent<Animator>();
        sr = signSprite.GetComponent<SpriteRenderer>();
        playerInput = new PlayerInputController();
        playerInput.Enable();
    }
    private void OnEnable()
    {
        playerInput.Gameplay.Confirm.started += OnComfirm;
    }
    private void Update()
    {
        sr.enabled = canPress;
        signSprite.transform.localScale = playerTrans.localScale;
    }
    private void OnComfirm(InputAction.CallbackContext context)
    {
        if (canPress && targetItem != null)
        {
            targetItem.TriggerAction();
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
        {
            canPress = true;
            targetItem = other.GetComponent<IInteractable>();
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
