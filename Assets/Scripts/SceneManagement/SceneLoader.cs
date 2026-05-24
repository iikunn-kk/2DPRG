using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoader : MonoBehaviour
{
    //单例模式，并且不销毁
    public static SceneLoader Instance { get; private set; }

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
        DontDestroyOnLoad(gameObject);//加载新场景时告诉游戏引擎，不要销毁该对象
    }

    public void TransitionToScene(string sceneName)
    {
        if (sceneName == null)
        {
            Debug.Log("场景为空");
        }
        else
        {
            StartCoroutine(TransitionCoroutine(sceneName));
        }
    }

    public IEnumerator TransitionCoroutine(string newSceneName)
    {
        // //淡出当前场景
        // yield return StartCoroutine(ScreenFader.Instance.FadeSceneOut());
        SceneFader fade;
        yield return fade = FindFirstObjectByType<SceneFader>();//寻找场景淡入淡出物体
        yield return StartCoroutine(fade.FadeOut(2f));// 执行淡出效果

        //异步加载场景
        yield return SceneManager.LoadSceneAsync(newSceneName);
        //加载所有持久化数据

        //获取目标场景过渡的位置
        // SceneEntrance entrance = FindFirstObjectByType<SceneEntrance>();
        //设置进入游戏对象的位置
        // SetEnteringPosition(entrance);


        yield return StartCoroutine(fade.FadeIn(2f));// 执行淡入效果
        // //淡入新场景
        // yield return StartCoroutine(ScreenFader.Instance.FadeSceneIn());
    }

    // private void SetEnteringPosition(SceneEntrance entrance)
    // {
    //     //把目标场景过渡的位置赋给玩家的位置
    //     Transform entranceTransform = entrance.transform;
    //     Player.Instance.transform.position = entranceTransform.position;
    // }
}
