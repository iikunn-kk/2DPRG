using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景加载器 - 单例模式
/// 负责场景异步加载与淡入淡出过渡效果
/// </summary>
public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [Header("过渡设置")]
    [SerializeField] private float _fadeDuration = 2f;

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
    /// 过渡到指定场景（带淡入淡出效果）
    /// </summary>
    public void TransitionToScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("[SceneLoader] 场景名称为空，无法过渡");
            return;
        }
        StartCoroutine(TransitionCoroutine(sceneName));
    }

    private IEnumerator TransitionCoroutine(string newSceneName)
    {
        // 使用单例引用（避免 FindFirstObjectByType 在异步加载后返回已销毁实例）
        var fade = SceneFader.Instance;
        if (fade == null)
        {
            Debug.LogWarning("[SceneLoader] 未找到 SceneFader，直接加载场景");
            yield return SceneManager.LoadSceneAsync(newSceneName);
            yield break;
        }

        yield return StartCoroutine(fade.FadeOut(_fadeDuration));
        yield return SceneManager.LoadSceneAsync(newSceneName);

        // 等待 2 帧让新场景 Awake/Start 风暴完成，避免第一帧卡顿吃掉淡入效果
        yield return null;
        yield return null;

        yield return StartCoroutine(fade.FadeIn(_fadeDuration));
    }
}
