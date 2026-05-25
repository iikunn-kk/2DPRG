using UnityEngine;

/// <summary>
/// 单例模式基类（MonoBehaviour 类型）
/// 用于需要挂载在 GameObject 上的管理器类
/// </summary>
/// <typeparam name="T">继承自 Singleton 的类类型</typeparam>
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;
    private static readonly object lockObject = new object();

    /// <summary>
    /// 获取单例实例（线程安全）
    /// </summary>
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                // 尝试在场景中查找已存在的实例
                instance = FindFirstObjectByType<T>();

                if (instance == null)
                {
                    Debug.LogError($"[Singleton] 未找到 {typeof(T).Name} 的实例！请确保已在场景中创建该对象。");
                }
                else
                {
                    Debug.LogWarning($"[Singleton] {typeof(T).Name} 实例未通过 Awake 初始化，已通过 FindObjectOfType 查找。");
                }
            }

            return instance;
        }
    }

    protected virtual void Awake()
    {
        lock (lockObject)
        {
            if (instance != null && instance != this)
            {
                Debug.LogWarning($"[Singleton] 场景中已存在 {typeof(T).Name} 的实例，销毁重复的实例。");
                Destroy(gameObject);
                return;
            }

            instance = (T)this;
        }
    }

    /// <summary>
    /// 检查单例是否已初始化
    /// </summary>
    public static bool IsInitialized
    {
        get { return instance != null; }
    }

    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
