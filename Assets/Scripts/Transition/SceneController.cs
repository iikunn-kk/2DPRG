using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        // 初始化逻辑（如有需要可在此添加）
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
        // 保存数据
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SavePlayerData();// 保存当前场景数据
            UnityEngine.Debug.Log("上一场景数据保存完毕");
        }
        else
        {
            UnityEngine.Debug.LogWarning("SaveManager.Instance 为空，跳过保存");
        }

        if (SceneManager.GetActiveScene().name != sceneName)//不同场景的加载
        {
            UnityEngine.Debug.Log("玩家当前所在的场景为：" + SceneManager.GetActiveScene().name);


            // 获取当前场景的 SceneFader（使用单例引用，避免 FindFirstObjectByType 返回已销毁实例）
            SceneFader fade = SceneFader.Instance;
            if (fade == null)
            {
                UnityEngine.Debug.LogError("未找到 SceneFader，无法执行场景过渡");
                yield break;
            }

            yield return StartCoroutine(fade.FadeOut(fadeDuration));// 执行淡出效果

            yield return SceneManager.LoadSceneAsync(sceneName);// 异步加载新场景

            // 等待 4 帧让新场景 Awake/Start 风暴完成，避免首帧卡顿
            yield return null;
            yield return null;
            yield return null;
            yield return null;

            UnityEngine.Debug.Log("新场景" + sceneName + "加载完毕");

            // 【关键修复】场景加载后使用单例引用（避免获取到即将被销毁的重复实例）
            fade = SceneFader.Instance;
            if (fade == null)
            {
                UnityEngine.Debug.LogError("新场景中未找到 SceneFader");
                yield break;
            }

            // 获取目标传送点（添加 null 检查）
            TransitionDestination destination = GetDestination(destinationTag);
            if (destination == null)
            {
                UnityEngine.Debug.LogError($"未找到目标传送点: {destinationTag}");
                yield break;
            }

            yield return Instantiate(playerPrefab, destination.transform.position, destination.transform.rotation);// 在新场景生成玩家

            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.LoadPlayerData();            //加载上一个场景保存的数据
                UnityEngine.Debug.Log("新场景" + sceneName + "的数据加载完毕");
            }

            yield return StartCoroutine(fade.FadeIn(fadeDuration));// 执行淡入效果

            yield break;
        }
        else//相同场景内不同位置的加载
        {
            // 直接更新玩家位置（添加 null 检查）
            if (GameManager.Instance == null || GameManager.Instance.characterStats == null)
            {
                UnityEngine.Debug.LogError("GameManager.Instance 或 characterStats 为空");
                yield break;
            }

            player = GameManager.Instance.characterStats.gameObject;
            TransitionDestination destination = GetDestination(destinationTag);

            if (destination == null)
            {
                UnityEngine.Debug.LogError($"未找到目标传送点: {destinationTag}");
                yield break;
            }

            player.transform.SetPositionAndRotation(destination.transform.position, destination.transform.rotation);
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
    [Header("场景过渡设置")]
    [SerializeField] private float fadeDuration = 2f; // 淡入淡出持续时间

    public void TransitionToFirstLevel()//主场景的新游戏选项执行的功能函数
    {
        StartCoroutine(LoadLevel(FirstLevelSceneName));
    }

    public void TransitionToLoadGame()//主场景的继续游戏选项执行的功能函数
    {
        if (SaveManager.Instance != null)
        {
            StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));//继续游戏的协程
        }
        else
        {
            Debug.LogError("SaveManager.Instance 为空，无法加载游戏");
        }
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
        SceneFader fade = FindFirstObjectByType<SceneFader>();

        if (string.IsNullOrEmpty(scene))
        {
            Debug.LogError("场景名称为空");
            yield break;
        }

        if (fade != null)
        {
            yield return StartCoroutine(fade.FadeOut(fadeDuration));//淡出场景
        }

        yield return SceneManager.LoadSceneAsync(scene);//异步加载场景
        UnityEngine.Debug.Log(scene + "场景加载完毕");

        if (GameManager.Instance != null && GameManager.Instance.GetEntrance() != null)
        {
            yield return player = Instantiate(playerPrefab, GameManager.Instance.GetEntrance().position, GameManager.Instance.GetEntrance().rotation);//实例化一个新的玩家
            //在PlayerController脚本中的OnEnable函数中，玩家自行拿到上一次保存的人物属性的数据
            UnityEngine.Debug.Log("人物实例化完毕");
        }
        else
        {
            Debug.LogError("GameManager.Instance 或 GetEntrance() 为空，无法实例化玩家");
        }

        if (fade != null)
        {
            yield return StartCoroutine(fade.FadeIn(fadeDuration));//淡入场景
        }
        yield break;
    }
    IEnumerator LoadMain()//返回主场景携程
    {
        SceneFader fade = FindFirstObjectByType<SceneFader>();//淡入淡出面板

        if (SaveManager.Instance != null)
        {
            PlayerPrefs.SetString(SaveManager.Instance.sceneName, SceneManager.GetActiveScene().name); // 保存当前场景
            if (PlayerController.Instance != null && !PlayerController.Instance.isDead)//只有人物非死亡状态下返回主场景才保存人物数据
            {
                SaveManager.Instance.SavePlayerData();//保存人物属性    
            }
        }

        UnityEngine.Debug.Log("当前保存的场景名字是：" + SceneManager.GetActiveScene().name);
        UnityEngine.Debug.Log("返回主场景，人物当前场景数据保存完毕");

        if (fade != null)
        {
            yield return StartCoroutine(fade.FadeOut(fadeDuration));//淡出
            yield return SceneManager.LoadSceneAsync(MainSceneName);//返回主场景
            yield return StartCoroutine(fade.FadeIn(fadeDuration));//淡入
        }
        else
        {
            yield return SceneManager.LoadSceneAsync(MainSceneName);//返回主场景（无淡入淡出效果）
        }

        yield break;
    }

    IEnumerator TheFinalLoadMain()
    {
        UnityEngine.Debug.Log("返回主场景");
        SceneFader fade = FindFirstObjectByType<SceneFader>();

        if (fade != null)
        {
            yield return StartCoroutine(fade.FadeOut(fadeDuration));
            yield return SceneManager.LoadSceneAsync(MainSceneName);
            yield return StartCoroutine(fade.FadeIn(fadeDuration));
        }
        else
        {
            yield return SceneManager.LoadSceneAsync(MainSceneName);
        }

        yield break;
    }
    IEnumerator RestartGameCoroutine()
    {
        SceneFader fade = FindFirstObjectByType<SceneFader>();//淡入淡出面板

        if (SaveManager.Instance != null)
        {
            PlayerPrefs.SetString(SaveManager.Instance.sceneName, SceneManager.GetActiveScene().name); // 保存当前场景
        }

        if (fade != null)
        {
            yield return StartCoroutine(fade.FadeOut(fadeDuration));//淡出场景
        }

        if (SaveManager.Instance != null)
        {
            yield return SceneManager.LoadSceneAsync(SaveManager.Instance.SceneName);//异步加载场景
            UnityEngine.Debug.Log(SaveManager.Instance.SceneName + "场景加载完毕");
        }

        if (GameManager.Instance != null && GameManager.Instance.GetEntrance() != null)
        {
            yield return player = Instantiate(playerPrefab, GameManager.Instance.GetEntrance().position, GameManager.Instance.GetEntrance().rotation);//实例化一个新的玩家//在PlayerController脚本中的OnEnable函数中，玩家自行拿到上一次保存的人物属性的数据
            UnityEngine.Debug.Log("人物实例化完毕");
        }

        if (fade != null)
        {
            yield return StartCoroutine(fade.FadeIn(fadeDuration));//淡入场景
        }

        yield break;
    }

}

