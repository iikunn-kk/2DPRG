using UnityEngine;

/// <summary>
/// 元素对话 UI 显示控制
/// 激活/显示元素对话面板
/// </summary>
public class AppearElementDialogue : MonoBehaviour
{
    [Header("对话UI")]
    [SerializeField] private GameObject _elementDialogue;

    /// <summary>
    /// 激活对话元素（供 UnityEvent / Animation Event 调用）
    /// </summary>
    public void SetTrue()
    {
        if (_elementDialogue != null)
            _elementDialogue.SetActive(true);
    }
}
