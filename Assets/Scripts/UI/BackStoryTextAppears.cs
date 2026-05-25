using UnityEngine;

/// <summary>
/// 背景故事文本显示
/// 文本显示完成后自动隐藏 UI
/// </summary>
public class BackStoryTextAppears : BaseTextAppears
{
    protected override void OnTextComplete()
    {
        HideObject(); // 简单隐藏（背景故事只显示一次）
    }
}
