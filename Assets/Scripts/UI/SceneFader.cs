using System.Collections;
using UnityEngine;

/// <summary>
/// 场景淡入淡出管理器（单例，跨场景持久化）
/// 控制场景切换时的 CanvasGroup alpha 渐变效果
/// </summary>
public class SceneFader : Singleton<SceneFader>
{
    [Header("淡入淡出配置")]
    [SerializeField] private float fadeInDuration = 1.5f;
    [SerializeField] private float fadeOutDuration = 1.5f;

    private CanvasGroup _canvasGroup;
    private Canvas _canvas;
    private bool _isValidInstance; // true = 这是被保留的单例，false = 即将被销毁

    protected override void Awake()
    {
        base.Awake();

        if (this == null) return;

        _canvasGroup = GetComponent<CanvasGroup>();
        _canvas = GetComponent<Canvas>();
        if (_canvasGroup == null)
        {
            Debug.LogError("[SceneFader] 未找到 CanvasGroup 组件！");
            return;
        }

        // 确保 Fade Canvas 始终渲染在最顶层（不被新场景 UI 遮挡）
        if (_canvas != null)
            _canvas.sortingOrder = 999;

        _canvasGroup.alpha = 0f;
        DontDestroyOnLoad(this);
        _isValidInstance = true;
    }

    /// <summary>
    /// 检查组件的有效性（避免在已销毁对象上调用）
    /// </summary>
    private bool IsValid()
    {
        // Unity 重载了 == null 来检测已销毁的对象
        return _isValidInstance && this != null && _canvasGroup != null;
    }

    public IEnumerator FadeOutIn()
    {
        yield return FadeOut(fadeOutDuration);
        yield return FadeIn(fadeInDuration);
    }

    /// <summary>
    /// 淡出到黑色（alpha: 0 → 1）
    /// </summary>
    public IEnumerator FadeOut(float duration)
    {
        if (!IsValid()) yield break;

        // 每次淡出前确保渲染层级最高（场景切换后可能被重置）
        if (_canvas != null)
            _canvas.sortingOrder = 999;

        float elapsed = 0f;
        _canvasGroup.alpha = 0f;

        while (_canvasGroup.alpha < 1f)
        {
            elapsed += Time.unscaledDeltaTime;
            _canvasGroup.alpha = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        _canvasGroup.alpha = 1f;
    }

    /// <summary>
    /// 淡入到透明（alpha: 1 → 0）
    /// </summary>
    public IEnumerator FadeIn(float duration)
    {
        if (!IsValid()) yield break;

        float elapsed = 0f;
        _canvasGroup.alpha = 1f;

        while (_canvasGroup.alpha > 0f)
        {
            elapsed += Time.unscaledDeltaTime;
            _canvasGroup.alpha = Mathf.Clamp01(1f - (elapsed / duration));
            yield return null;
        }

        _canvasGroup.alpha = 0f;
    }

    public void SetBlack()
    {
        if (IsValid())
            _canvasGroup.alpha = 1f;
    }

    public void SetTransparent()
    {
        if (IsValid())
            _canvasGroup.alpha = 0f;
    }
}
