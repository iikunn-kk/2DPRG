using UnityEngine;
using NodeCanvas.DialogueTrees;

/// <summary>
/// 对话启动器
/// 当对象激活时自动开始对话
/// </summary>
public class StartDialogue : MonoBehaviour
{
    [Header("对话配置")]
    [SerializeField] private DialogueTreeController _dialogueTree; // NodeCanvas 对话树控制器

    private void Awake()
    {
        // 自动获取组件（如果未手动拖入）
        if (_dialogueTree == null)
        {
            _dialogueTree = GetComponent<DialogueTreeController>();
            
            if (_dialogueTree == null)
            {
                Debug.LogWarning($"[StartDialogue] 未设置 DialogueTreeController: {gameObject.name}");
            }
        }
    }

    /// <summary>
    /// 开始对话（可由外部调用或绑定到 UI 按钮）
    /// </summary>
    public void Talk()
    {
        if (_dialogueTree == null)
        {
            Debug.LogError($"[StartDialogue] DialogueTreeController 为空: {gameObject.name}");
            return;
        }

        _dialogueTree.StartDialogue();
        
        Debug.Log($"[StartDialogue] 对话已启动: {gameObject.name}");
    }
}
