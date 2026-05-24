using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneFader : Singleton<SceneFader>
{
    CanvasGroup canvasGroup;
    public float fadeInDuration;
    public float fadeOutDuration;

    protected override void Awake()
    {
        base.Awake();
        canvasGroup = GetComponent<CanvasGroup>();
        DontDestroyOnLoad(this);
    }
    // new void Awake()
    // {
    //     canvasGroup = GetComponent<CanvasGroup>();
    //     DontDestroyOnLoad(gameObject);
    // }
    public IEnumerator FadeOutIn()//淡出淡入先出再入
    {
        yield return FadeOut(fadeOutDuration);
        yield return FadeIn(fadeInDuration);
    }

    public IEnumerator FadeOut(float time)
    {
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime / time;
            yield return null;
        }
    }
    public IEnumerator FadeIn(float time)
    {
        while (canvasGroup.alpha != 0)
        {
            canvasGroup.alpha -= Time.deltaTime / time;
            yield return null;
        }
    }
}
