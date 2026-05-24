using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
public class SceneController : Singleton<SceneController>
{
    private const string FirstLevelSceneName = "Training Ground";
    private const string MainSceneName = "Main";

    public GameObject playerPrefab;    // 玩家预制体引用
    public SceneFader sceneFaderPrefab;    // 场景切换淡入淡出预制体引用

    GameObject player;//当前玩家对象实例

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this); // 保持跨场景存活
    }

    void Start()
    {
        // GameObject player = GameManager.Instance.characterStats.gameObject;
    }
    // 场景传送入口方法
    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        switch (transitionPoint.transitionType)
        {
            case TransitionPoint.TransitionType.SameScene:
                UnityEngine.Debug.Log("同场景传送");
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
                break;
            case TransitionPoint.TransitionType.DifferentScene:
                StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationTag));
                break;
        }
    }
    // 场景过渡协程（核心逻辑）
    public IEnumerator Transition(string sceneName, TransitionDestination.DestinationTag destinationTag)
    {
        // GameObject player = GameManager.Instance.characterStats.gameObject;

        //保存数据
        SaveManager.Instance.SavePlayerData();// 保存当前场景数据
        UnityEngine.Debug.Log("上一场景数据保存完毕");
        if (SceneManager.GetActiveScene().name != sceneName)//不同场景的加载
        {
            UnityEngine.Debug.Log("玩家当前所在的场景为：" + SceneManager.GetActiveScene().name);
            // SceneFader fade = Instantiate(sceneFaderPrefab);
            // SceneFader fade = FindFirstObjectByType<SceneFader>();
            SceneFader fade;
            yield return fade = FindFirstObjectByType<SceneFader>();//寻找场景淡入淡出物体
            yield return StartCoroutine(fade.FadeOut(2f));// 执行淡出效果

            yield return SceneManager.LoadSceneAsync(sceneName);// 异步加载新场景
            UnityEngine.Debug.Log("新场景" + sceneName + "加载完毕");


            yield return Instantiate(playerPrefab, GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);// 在新场景生成玩家

            SaveManager.Instance.LoadPlayerData();            //加载上一个场景保存的数据

            UnityEngine.Debug.Log("新场景" + sceneName + "的数据加载完毕");

            yield return StartCoroutine(fade.FadeIn(2f));// 执行淡入效果

            yield break;
        }
        else//相同场景内不同位置的加载
        {
            // 直接更新玩家位置
            player = GameManager.Instance.characterStats.gameObject;
            player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            yield return null;

        }
    }
    // 获取场景中的目标传送点
    private TransitionDestination GetDestination(TransitionDestination.DestinationTag destinationTag)
    {
        // 遍历场景中的所有目标传送点
        var entrances = FindObjectsByType<TransitionDestination>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < entrances.Length; i++)
        {
            if (entrances[i].destinationTag == destinationTag)
                return entrances[i];
        }
        return null;
    }
    public void TransitionToFirstLevel()//主场景的新游戏选项执行的功能函数
    {
        StartCoroutine(LoadLevel(FirstLevelSceneName));
    }

    public void TransitionToLoadGame()//主场景的继续游戏选项执行的功能函数
    {

        StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));//继续游戏的协程
    }

    public void TransitionToMain()//返回主场景的函数(在功能面板下返回主场景)
    {
        StartCoroutine(LoadMain());//返回主场景的协程
    }
    public void TheFinalSceneTransitionToMain()
    {
        StartCoroutine(TheFinalLoadMain());//返回主场景的协程
    }
    public void RestartGameScene()
    {
        StartCoroutine(RestartGameCoroutine());//重新开始游戏的协程
    }
    /// <summary>
    /// 加载游戏关卡协程
    /// </summary>
    /// <param name="scene">目标关卡名称</param>
    IEnumerator LoadLevel(string scene)
    {
        UnityEngine.Debug.Log("当前加载的场景是" + scene);
        // SceneFader fade = Instantiate(sceneFaderPrefab);
        SceneFader fade = FindFirstObjectByType<SceneFader>();
        if (scene != "")
        {
            yield return StartCoroutine(fade.FadeOut(2f));//淡出场景
            yield return SceneManager.LoadSceneAsync(scene);//异步加载场景
            UnityEngine.Debug.Log(scene + "场景加载完毕");
            yield return player = Instantiate(playerPrefab, GameManager.Instance.GetEntrance().position, GameManager.Instance.GetEntrance().rotation);//实例化一个新的玩家
            //在PlayerController脚本中的OnEnable函数中，玩家自行拿到上一次保存的人物属性的数据
            UnityEngine.Debug.Log("人物实例化完毕");
            // SaveManager.Instance.SavePlayerData();//保存数据
            yield return StartCoroutine(fade.FadeIn(2f));//淡入场景
            yield break;
        }
    }
    IEnumerator LoadMain()//返回主场景携程
    {
        SceneFader fade = FindFirstObjectByType<SceneFader>();//淡入淡出面板
        PlayerPrefs.SetString(SaveManager.Instance.sceneName, SceneManager.GetActiveScene().name); // 保存当前场景
        if (!PlayerController.Instance.isDead)//只有人物非死亡状态下返回主场景才保存人物数据
        {
            SaveManager.Instance.SavePlayerData();//保存人物属性    
        }
        UnityEngine.Debug.Log("当前保存的场景名字是：" + SceneManager.GetActiveScene().name);
        UnityEngine.Debug.Log("返回主场景，人物当前场景数据保存完毕");
        yield return StartCoroutine(fade.FadeOut(2f));//淡出
        yield return SceneManager.LoadSceneAsync(MainSceneName);//返回主场景
        yield return StartCoroutine(fade.FadeIn(2f));//淡入
        yield break;
    }

    IEnumerator TheFinalLoadMain()
    {
        UnityEngine.Debug.Log("返回主场景");
        SceneFader fade = FindFirstObjectByType<SceneFader>();
        yield return StartCoroutine(fade.FadeOut(2f));
        yield return SceneManager.LoadSceneAsync(MainSceneName);
        yield return StartCoroutine(fade.FadeIn(2f));
        yield break;
    }
    IEnumerator RestartGameCoroutine()
    {
        SceneFader fade = FindFirstObjectByType<SceneFader>();//淡入淡出面板
        PlayerPrefs.SetString(SaveManager.Instance.sceneName, SceneManager.GetActiveScene().name); // 保存当前场景
        yield return StartCoroutine(fade.FadeOut(2f));//淡出场景
        yield return SceneManager.LoadSceneAsync(SaveManager.Instance.SceneName);//异步加载场景
        UnityEngine.Debug.Log(SaveManager.Instance.SceneName + "场景加载完毕");
        yield return player = Instantiate(playerPrefab, GameManager.Instance.GetEntrance().position, GameManager.Instance.GetEntrance().rotation);//实例化一个新的玩家//在PlayerController脚本中的OnEnable函数中，玩家自行拿到上一次保存的人物属性的数据
        UnityEngine.Debug.Log("人物实例化完毕");
        yield return StartCoroutine(fade.FadeIn(2f));//淡入场景
        yield break;
    }

}

