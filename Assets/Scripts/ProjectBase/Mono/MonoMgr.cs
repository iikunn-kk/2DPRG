using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Mono 管理器 - 单例模式
/// 提供帧更新事件注册和协程托管服务
/// </summary>
public class MonoMgr : BaseManager<MonoMgr>
{
    private MonoController _controller;

    public MonoMgr()
    {
        var obj = new GameObject("MonoController");
        _controller = obj.AddComponent<MonoController>();
    }

    public void AddUpdateListener(UnityAction fun)
    {
        _controller.AddUpdateListener(fun);
    }

    public void RemoveUpdateListener(UnityAction fun)
    {
        _controller.RemoveUpdateListener(fun);
    }

    public Coroutine StartCoroutine(IEnumerator routine)
    {
        return _controller.StartCoroutine(routine);
    }

    public void StopCoroutine(IEnumerator routine)
    {
        _controller.StopCoroutine(routine);
    }
}
