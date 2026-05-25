using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using NodeCanvas.DialogueTrees;

/// <summary>
/// 可交互对话组件
/// 当玩家靠近并按下交互键时，触发 NPC 对话
/// </summary>
public class InteractableTalk : MonoBehaviour
{
    [Header("对话配置")]
    [FormerlySerializedAs("dialogue")]
    [SerializeField] private DialogueTreeController _dialogue; // NodeCanvas 对话树控制器

    // 运行时状态
    private HUD _hud;
    private bool _isPressDialogue;      // 当前是否按下对话键
    private bool _dialogueIsRunning;    // 对话是否正在运行
    private bool _wasDialogueRunning;   // 上一帧对话状态

    public bool DialogueIsRunning => _dialogueIsRunning;

    void Update()
    {
        InitializeHUDIfNeeded();
        
        UpdateDialogueState();
        HandlePlayerInput();
        HandleDialogueUI();
    }

    /// <summary>
    /// 延迟初始化 HUD 引用（避免 Awake 时找不到）
    /// </summary>
    private void InitializeHUDIfNeeded()
    {
        if (_hud == null)
        {
            _hud = FindFirstObjectByType<HUD>();
        }
    }

    /// <summary>
    /// 更新对话运行状态
    /// </summary>
    private void UpdateDialogueState()
    {
        if (_dialogue != null)
        {
            _wasDialogueRunning = _dialogueIsRunning;
            _dialogueIsRunning = _dialogue.isRunning;
        }
    }

    /// <summary>
    /// 对话结束时自动恢复 Talk UI
    /// </summary>
    private void HandleDialogueUI()
    {
        if (_wasDialogueRunning && !_dialogueIsRunning)
        {
            ShowInteractUI();
        }
    }

    /// <summary>
    /// 处理玩家输入（交互键）
    /// </summary>
    private void HandlePlayerInput()
    {
        if (_hud == null) return;

        if (_hud.playerInputController.Gameplay.Interact.WasPerformedThisFrame())
        {
            _isPressDialogue = true;
        }
        
        if (_hud.playerInputController.Gameplay.Interact.WasReleasedThisFrame())
        {
            _isPressDialogue = false;
        }
    }

    #region 碰撞事件

    /// <summary>
    /// 玩家进入交互范围 → 显示交互提示
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ShowInteractUI();
        }
    }

    /// <summary>
    /// 玩家在交互范围内 + 按下交互键 → 开始对话
    /// </summary>
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || !_isPressDialogue) return;
        if (_dialogueIsRunning) return;

        StartDialogue();
    }

    /// <summary>
    /// 玩家离开交互范围 → 隐藏交互提示
    /// </summary>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HideInteractUI();
        }
    }

    #endregion

    #region 辅助方法

    /// <summary>
    /// 显示交互 UI 提示
    /// </summary>
    private void ShowInteractUI()
    {
        if (_hud?.interactableUI != null)
        {
            _hud.interactableUI.SetActive(true);
        }
    }

    /// <summary>
    /// 隐藏交互 UI 提示
    /// </summary>
    private void HideInteractUI()
    {
        if (_hud?.interactableUI != null)
        {
            _hud.interactableUI.SetActive(false);
        }
    }

    /// <summary>
    /// 启动对话
    /// </summary>
    private void StartDialogue()
    {
        if (_dialogue == null)
        {
            Debug.LogError($"[InteractableTalk] 未设置对话控制器: {gameObject.name}");
            return;
        }

        _dialogue.StartDialogue();
        HideInteractUI(); // 隐藏交互提示
        
        // 通知游戏管理器暂停玩家移动
        DialogueTreeManager.Instance.OndialogueStopMove();
    }

    #endregion
}
