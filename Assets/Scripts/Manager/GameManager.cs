using UnityEngine;

/// <summary>
/// 游戏核心管理类（单例模式）
/// 负责管理玩家角色数据和场景过渡逻辑
/// </summary>
public class GameManager : Singleton<GameManager>
{
    /// <summary>
    /// 静态快捷访问属性 - 当前玩家角色数据
    /// </summary>
    public static CharacterStats PlayerStats => Instance.characterStats;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this); // 保持跨场景不销毁
    }

    /// <summary>
    /// 当前管理的玩家角色数据引用
    /// </summary>
    public CharacterStats characterStats;

    /// <summary>
    /// 注册玩家角色到游戏管理器
    /// </summary>
    /// <param name="player">需要注册的角色数据</param>
    public void RegisterPlayer(CharacterStats player)
    {
        characterStats = player; // 更新当前角色引用
    }

    /// <summary>
    /// 获取场景入口位置
    /// </summary>
    /// <returns>标有ENTER标记的过渡目标位置</returns>
    public Transform GetEntrance()
    {
        // 遍历场景中所有过渡目标组件
        foreach (var item in FindObjectsByType<TransitionDestination>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (item.destinationTag == TransitionDestination.DestinationTag.ENTER)
                return item.transform; // 返回首个入口点
        }
        return null;
    }
}
