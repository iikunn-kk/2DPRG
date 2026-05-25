using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using NodeCanvas.DialogueTrees;

/// <summary>
/// 可拾取物品对话组件
/// 当玩家靠近可拾取物品并按下交互键时，触发对话
/// </summary>
public class PickUpTalk : MonoBehaviour
{
    [Header("对话配置")]
    [FormerlySerializedAs("dialogue")]
    [SerializeField] private DialogueTreeController _dialogue; // NodeCanvas 对话树控制器

    // 运行时状态
    private HUD _hud;
    private bool _isPressDialogue;
    private bool _dialogueIsRunning;
    private bool _wasDialogueRunning;

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
    /// 只查找一次，后续缓存使用
    /// </summary>
    private void InitializeHUDIfNeeded()
    {
        if (_hud == null)
        {
            _hud = FindFirstObjectByType<HUD>();

            if (_hud == null)
            {
                // 仅在首次未找到时输出一次警告（避免日志刷屏）
                // Debug.LogWarning($"[PickUpTalk] HUD 未找到: {gameObject.name}");
            }
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
            ShowPickupUI();
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
    /// 玩家进入交互范围 → 显示拾取提示
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ShowPickupUI();
        }
    }

    /// <summary>
    /// 玩家在交互范围内 + 按下交互键 → 开始对话
    /// </summary>
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || !_isPressDialogue) return;
        if (_dialogueIsRunning) return;
        if (_hud == null) return;

        StartDialogue();
        HidePickupUI();
    }

    /// <summary>
    /// 玩家离开交互范围 → 隐藏提示
    /// </summary>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HidePickupUI();
        }
    }

    #endregion

    #region 辅助方法

    /// <summary>
    /// 显示拾取 UI 提示
    /// </summary>
    private void ShowPickupUI()
    {
        if (_hud?.pickupUI != null)
        {
            _hud.pickupUI.SetActive(true);
        }
    }

    /// <summary>
    /// 隐藏拾取 UI 提示
    /// </summary>
    private void HidePickupUI()
    {
        if (_hud?.pickupUI != null)
        {
            _hud.pickupUI.SetActive(false);
        }
    }

    /// <summary>
    /// 启动对话
    /// </summary>
    private void StartDialogue()
    {
        if (_dialogue == null)
        {
            Debug.LogError($"[PickUpTalk] 未设置对话控制器: {gameObject.name}");
            return;
        }

        _dialogue.StartDialogue();

        // 通知游戏管理器暂停玩家移动
        DialogueTreeManager.Instance.OndialogueStopMove();
    }

    #endregion
}
