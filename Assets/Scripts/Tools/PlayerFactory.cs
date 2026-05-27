using UnityEngine;

/// <summary>
/// 玩家工厂 — 简单工厂模式 + 生成前后钩子
/// 统一管理玩家实例化，保证事件触发顺序不变。
/// 
/// 生成流程：
///   1. Instantiate(prefab, position, rotation)
///   2. Unity 自动触发 Awake → OnEnable → Start（顺序不变）
///   3. 返回玩家 GameObject 供调用方继续处理
/// </summary>
public class PlayerFactory : Singleton<PlayerFactory>
{
    [Header("玩家预制体")]
    [SerializeField] private GameObject _playerPrefab;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// 生成玩家实例
    /// 不接管 Awake/OnEnable/Start — Unity 会自动按原有顺序触发。
    /// </summary>
    /// <param name="position">生成位置</param>
    /// <param name="rotation">生成朝向</param>
    /// <returns>玩家 GameObject（已挂载所有组件）</returns>
    public GameObject CreatePlayer(Vector3 position, Quaternion rotation)
    {
        if (_playerPrefab == null)
        {
            Debug.LogError("[PlayerFactory] 玩家预制体未设置！");
            return null;
        }

        var player = Instantiate(_playerPrefab, position, rotation);

        Debug.Log($"[PlayerFactory] 玩家已创建于 {position}");

        return player;
    }
}
