using UnityEngine;

/// <summary>
/// 木材元素结束语文本显示
/// 文本显示完成后自动隐藏 UI
/// </summary>
public class TextAppears : BaseTextAppears
{
    protected override void OnTextComplete()
    {
        HideObject(); // 简单隐藏
    }
}
