using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using NodeCanvas.DialogueTrees;
using NodeCanvas.DialogueTrees.UI.Examples;
using NodeCanvas.Tasks.Actions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PickUpTalk : MonoBehaviour
{
    HUD hud;
    // DialogueUGUI ugui;
    public bool isPressDialogue;
    public bool dialogueIsRunning;

    [SerializeField] DialogueTreeController dialogue;

    // public UnityEvent stopMove;
    // public UnityEvent startMove;
    void Awake()
    {

        // ugui = FindFirstObjectByType<DialogueUGUI>();
    }

    void Update()
    {
        if (hud == null)
        {
            hud = FindFirstObjectByType<HUD>();
        }
        dialogueIsRunning = dialogue.isRunning;//不断判断是否在对话状态
        if (hud != null)
        {
            if (hud.playerInputController.Gameplay.Interact.WasPerformedThisFrame())
            {
                isPressDialogue = true;
                //Debug.Log("按下了对话键");
            }
            if (hud.playerInputController.Gameplay.Interact.WasReleasedThisFrame())
            {
                isPressDialogue = false;
                //Debug.Log("松开了对话键");
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            hud.pickupUI.SetActive(true);
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        //如果玩家按下交互键，开始对话
        if (other.CompareTag("Player") && isPressDialogue)
        {
            if (!dialogueIsRunning)
            {
                // Debug.Log("触发对话");
                // Debug.Log("与你接触的物体是" + other.name);
                dialogue.StartDialogue();
                hud.pickupUI.SetActive(false);
                // stopMove?.Invoke();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        //  隐藏交互提示
        if (other.CompareTag("Player") && hud.interactableUI != null)
        {
            hud.interactableUI.SetActive(false);
        }
    }
    // public IEnumerator PostponeTest()
    // {
    //     yield return new WaitForSeconds(0.5f);
    // }
}
