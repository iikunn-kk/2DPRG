using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 泛型对象池，支持 MonoBehaviour 子类的复用。
/// 用于敌人、子弹、特效等频繁创建/销毁的对象，减少 GC 分配。
///
/// 使用方式：
///   var pool = new ObjectPool<Enemy>(prefab, initialSize: 5);
///   var enemy = pool.Get();          // 从池中取出（未激活时自动激活）
///   pool.Release(enemy);             // 放回池中（自动停用）
///   pool.Clear();                    // 清空池（切换场景时调用）
/// </summary>
public class ObjectPool<T> where T : Component
{
    private readonly T _prefab;
    private readonly Transform _parent;
    private readonly Queue<T> _pool = new Queue<T>();
    private readonly int _maxSize;

    public int ActiveCount { get; private set; }
    public int PooledCount => _pool.Count;
    public int TotalCount => ActiveCount + PooledCount;

    /// <param name="prefab">源预制体</param>
    /// <param name="initialSize">预热数量（场景加载时预先实例化）</param>
    /// <param name="maxSize">池容量上限（超出后不再缓存，直接 Destroy）</param>
    /// <param name="parent">可选父节点（用于 Hierarchy 整洁）</param>
    public ObjectPool(T prefab, int initialSize = 0, int maxSize = 50, Transform parent = null)
    {
        _prefab = prefab;
        _parent = parent;
        _maxSize = maxSize;

        // 预热
        for (int i = 0; i < initialSize; i++)
        {
            var obj = CreateNew();
            obj.gameObject.SetActive(false);
            _pool.Enqueue(obj);
        }
    }

    /// <summary>
    /// 从池中获取一个对象。自动激活。
    /// </summary>
    public T Get()
    {
        T obj = _pool.Count > 0 ? _pool.Dequeue() : CreateNew();

        obj.gameObject.SetActive(true);
        ActiveCount++;
        return obj;
    }

    /// <summary>
    /// 将对象放回池中。自动停用。
    /// </summary>
    public void Release(T obj)
    {
        if (obj == null) return;

        ActiveCount--;

        if (_pool.Count >= _maxSize)
        {
            // 池已满，直接销毁
            Object.Destroy(obj.gameObject);
            return;
        }

        obj.gameObject.SetActive(false);
        _pool.Enqueue(obj);
    }

    /// <summary>
    /// 清空池中所有缓存对象（切换场景时调用）。
    /// </summary>
    public void Clear()
    {
        while (_pool.Count > 0)
        {
            var obj = _pool.Dequeue();
            if (obj != null)
                Object.Destroy(obj.gameObject);
        }
        ActiveCount = 0;
    }

    private T CreateNew()
    {
        var obj = Object.Instantiate(_prefab, _parent);
        obj.name = _prefab.name; // 去掉 "(Clone)" 后缀
        return obj;
    }
}
