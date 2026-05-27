using UnityEngine;

/// <summary>
/// 掉落物工厂 — 简单工厂模式
/// 统一管理所有掉落物的创建逻辑。
/// 后续加对象池、改掉落物类型，只改这一个文件。
/// </summary>
public class ItemFactory : Singleton<ItemFactory>
{
    [Header("掉落物预制体")]
    [SerializeField] private GameObject _healthBottlePrefab;

    [Header("生成配置")]
    [SerializeField] private float _autoDestroyDelay = 30f; // 掉落物自动销毁时间

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// 生成一个血瓶
    /// </summary>
    /// <param name="position">生成位置</param>
    public GameObject CreateHealthBottle(Vector3 position)
    {
        if (_healthBottlePrefab == null)
        {
            Debug.LogWarning("[ItemFactory] 血瓶预制体未设置");
            return null;
        }

        var bottle = Instantiate(_healthBottlePrefab, position, Quaternion.identity);

        // 自动销毁（防止掉落物堆积）
        Destroy(bottle, _autoDestroyDelay);


        return bottle;
    }
}
