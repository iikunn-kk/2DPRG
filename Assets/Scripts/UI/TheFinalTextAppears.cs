using UnityEngine;

/// <summary>
/// 最终场景结束语文本显示
/// 文本显示完成后自动返回主场景
/// </summary>
public class TheFinalTextAppears : BaseTextAppears
{
    protected override void OnTextComplete()
    {
        // 返回主场景
        if (SceneController.Instance != null)
        {
            SceneController.Instance.TheFinalSceneTransitionToMain();
        }
        else
        {
            Debug.LogError("[TheFinalTextAppears] SceneController.Instance 为空");
        }

        HideObject();
    }
}
