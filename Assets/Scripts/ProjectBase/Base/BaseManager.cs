using System;
using UnityEngine;

/// <summary>
/// 普通类单例基类（非 MonoBehaviour）
/// 用于不需要继承 MonoBehaviour 的管理器类（如 EventCenter）
/// 
/// 使用方式：public class MyManager : BaseManager&lt;MyManager&gt; { }
/// </summary>
/// <typeparam name="T">继承自 BaseManager 的类类型</typeparam>
public class BaseManager<T> where T : class, new()
{
    private static T _instance;
    private static readonly object _lock = new object();

    /// <summary>
    /// 获取单例实例
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                    }
                }
            }
            return _instance;
        }
    }

    /// <summary>
    /// 检查单例是否已初始化
    /// </summary>
    public static bool IsInitialized => _instance != null;

    /// <summary>
    /// 重置单例（测试用或特殊场景切换时使用）
    /// </summary>
    protected static void ResetInstance()
    {
        lock (_lock)
        {
            _instance = null;
        }
    }
}
