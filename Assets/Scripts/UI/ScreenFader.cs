using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class ScreenFader : MonoBehaviour
{
    //单例模式

    public static ScreenFader Instance { get; private set; }
    public CanvasGroup faderCanvasGroup;
    public float fadeDuration = 1f;//淡入淡出时长

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != null)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);//将当前游戏对象设置为不销毁
    }

    //淡入场景
    public IEnumerator FadeSceneIn()
    {
        yield return StartCoroutine(Fade(0f, faderCanvasGroup));
        //禁用淡入淡出的CanvasGroup对象
        faderCanvasGroup.gameObject.SetActive(false);
    }
    //淡出场景
    public IEnumerator FadeSceneOut()
    {
        //启用淡入淡出的CanvasGroup对象
        faderCanvasGroup.gameObject.SetActive(true);
        yield return StartCoroutine(Fade(1f, faderCanvasGroup));
    }

    //淡入淡出实现
    public IEnumerator Fade(float finalAlpha, CanvasGroup canvasGroup)
    {
        yield return canvasGroup.DOFade(finalAlpha, fadeDuration).WaitForCompletion();
    }
}
