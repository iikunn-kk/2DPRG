using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AudioSource 对象池 — 避免频繁创建/销毁导致的 GC 分配
/// 
/// 使用方式：
///   var source = AudioSourcePool.Get();   // 获取一个可用 AudioSource
///   AudioSourcePool.Return(source);       // 用完归还
/// </summary>
public static class AudioSourcePool
{
    private static readonly Stack<AudioSource> _pool = new Stack<AudioSource>();
    private static GameObject _poolRoot; // 池中所有 AudioSource 的父对象

    private const int DefaultPoolSize = 8;
    private static bool _isInitialized;

    /// <summary>
    /// 初始化池（预创建指定数量）
    /// </summary>
    private static void Init(int initialSize = DefaultPoolSize)
    {
        if (_isInitialized) return;

        _poolRoot = new GameObject("[AudioSourcePool]");
        Object.DontDestroyOnLoad(_poolRoot);

        for (int i = 0; i < initialSize; i++)
        {
            Return(CreateNew());
        }

        _isInitialized = true;
        Debug.Log($"[AudioSourcePool] 初始化完成，预创建 {initialSize} 个 AudioSource");
    }

    /// <summary>
    /// 获取一个可用的 AudioSource（池中有则复用，没有则新建）
    /// </summary>
    public static AudioSource Get()
    {
        if (!_isInitialized) Init();

        if (_pool.Count > 0)
        {
            var source = _pool.Pop();
            source.gameObject.SetActive(true);
            return source;
        }

        // 池用完了，新建一个
        Debug.LogWarning("[AudioSourcePool] 池已耗尽，新建 AudioSource");
        return CreateNew();
    }

    /// <summary>
    /// 归还 AudioSource 到池中（停止播放、重置状态）
    /// </summary>
    public static void Return(AudioSource source)
    {
        if (source == null) return;

        // 停止播放并重置
        source.Stop();
        source.clip = null;
        source.loop = false;
        source.volume = 1f;
        source.gameObject.SetActive(false);
        source.transform.SetParent(_poolRoot.transform);

        _pool.Push(source);
    }

    /// <summary>
    /// 获取当前池中可用数量（调试用）
    /// </summary>
    public static int AvailableCount => _pool.Count;

    /// <summary>
    /// 清理整个池（场景切换时调用）
    /// </summary>
    public static void Clear()
    {
        while (_pool.Count > 0)
        {
            var source = _pool.Pop();
            if (source != null)
                Object.Destroy(source.gameObject);
        }

        if (_poolRoot != null)
            Object.Destroy(_poolRoot);

        _isInitialized = false;
    }

    private static AudioSource CreateNew()
    {
        var go = new GameObject("PooledAudio");
        go.transform.SetParent(_poolRoot?.transform);
        go.SetActive(false);
        return go.AddComponent<AudioSource>();
    }
}
