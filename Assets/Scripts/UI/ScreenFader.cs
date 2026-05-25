using System.Collections;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 画面淡入淡出控制器（使用 DOTween）
/// 单例模式，场景切换时不销毁
/// </summary>
public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance { get; private set; }

    [Header("淡入淡出设置")]
    [SerializeField] private CanvasGroup _faderCanvasGroup;
    [SerializeField] private float _fadeDuration = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 淡入场景（从黑到透明）
    /// </summary>
    public IEnumerator FadeSceneIn()
    {
        if (_faderCanvasGroup == null) yield break;

        yield return StartCoroutine(Fade(0f));
        _faderCanvasGroup.gameObject.SetActive(false);
    }

    /// <summary>
    /// 淡出场景（从透明到黑）
    /// </summary>
    public IEnumerator FadeSceneOut()
    {
        if (_faderCanvasGroup == null) yield break;

        _faderCanvasGroup.gameObject.SetActive(true);
        yield return StartCoroutine(Fade(1f));
    }

    /// <summary>
    /// 执行淡入淡出动画
    /// </summary>
    private IEnumerator Fade(float finalAlpha)
    {
        yield return _faderCanvasGroup.DOFade(finalAlpha, _fadeDuration).WaitForCompletion();
    }
}
