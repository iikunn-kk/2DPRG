using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class SaveManager : Singleton<SaveManager>
{
    // 存储场景名称的键值
    // string sceneName = "level";
    [HideInInspector] public string sceneName;
    // private PlayerInputController inputControl;

    /// <summary>
    /// 获取存储的场景名称（从PlayerPrefs读取）
    /// </summary>
    public string SceneName { get { return PlayerPrefs.GetString(sceneName); } }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this); // 保持跨场景不销毁
        // inputControl = new PlayerInputController();

        // inputControl.Gameplay.Save.started += SaveGameData;//保存游戏数据

        // inputControl.Gameplay.Load.started += LoadGameData;//加载游戏数据

        // inputControl.Gameplay.PausePanel.started += PauseGamePanel;//打开控制面板
    }


    // private void OnEnable()
    // {
    //     if (inputControl != null)
    //     {
    //         inputControl.Enable();
    //     }
    // }

    // private void OnDisable()
    // {
    //     if (inputControl != null)
    //     {
    //         inputControl.Disable();
    //     }
    // }
    void Update()
    {
        // // 按键事件处理
        // if (Input.GetKeyDown(KeyCode.Escape))
        // {
        //     SceneController.Instance.TransitionToMain();
        // }
        // if (Input.GetKeyDown(KeyCode.I)) // 保存快捷键
        // {
        //     SavePlayerData();
        //     Debug.Log("保存数据");
        // }
        // if (Input.GetKeyDown(KeyCode.O)) // 加载快捷键
        // {
        //     LoadPlayerData();
        //     Debug.Log("加载数据");
        // }
    }

    /// <summary>
    /// 保存玩家数据（调用通用保存方法）
    /// </summary>
    public void SavePlayerData()
    {
        Save(GameManager.Instance.characterStats.characterData,
            GameManager.Instance.characterStats.characterData.name);
    }

    /// <summary>
    /// 加载玩家数据（调用通用加载方法）
    /// </summary>
    public void LoadPlayerData()
    {
        Load(GameManager.Instance.characterStats.characterData,
            GameManager.Instance.characterStats.characterData.name);
    }

    /// <summary>
    /// 通用保存方法
    /// </summary>
    /// <param name="data">要保存的数据对象</param>
    /// <param name="key">存储键值</param>
    public void Save(Object data, string key)
    {
        var jsonData = JsonUtility.ToJson(data, true); // 序列化为JSON
        PlayerPrefs.SetString(key, jsonData); // 保存角色数据
        PlayerPrefs.SetString(sceneName, SceneManager.GetActiveScene().name); // 保存当前场景
        Debug.Log("当前保存的场景是：" + SceneManager.GetActiveScene().name);
        PlayerPrefs.Save(); // 立即写入磁盘
    }

    /// <summary>
    /// 通用加载方法
    /// </summary>
    /// <param name="data">要加载到的数据对象</param>
    /// <param name="key">存储键值</param>
    public void Load(Object data, string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), data); // 覆盖现有数据
        }
    }

    /// <summary>
    /// 测试用方法：增加角色最大生命值
    /// </summary>
    public void TestAddHealth()
    {
        GameManager.Instance.characterStats.MaxHealth += 100;
    }
    // private void SaveGameData(InputAction.CallbackContext context)
    // {
    //     SavePlayerData();
    //     Debug.Log("保存数据");
    // }
    // private void LoadGameData(InputAction.CallbackContext context)
    // {
    //     LoadPlayerData();
    //     Debug.Log("加载数据");
    // }
    // private void PauseGamePanel(InputAction.CallbackContext context)
    // {
    //     PausePanel.Instance.PauseGame();
    // }
}
