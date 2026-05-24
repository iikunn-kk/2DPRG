using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// 可以提供给外部添加帧更新事件的方法
///可以提供给外部添加 协程的方法
/// </summary>
public class MonoMgr : BaseManager<MonoMgr>
{
    private MonoController controller;

    public MonoMgr()
    {
        GameObject obj = new GameObject("MonoController");
        controller = obj.AddComponent<MonoController>();
    }

    public void AddUpdateListener(UnityAction fun)
    {
        controller.AddUpdateListener(fun);
    }
    /// <summary>
    /// 给外部移除帧更新事件的函数
    /// </summary>
    /// <param name="fun"></param>
    public void RemoveUpdateListener(UnityAction fun)
    {
        controller.RemoveUpdateListener(fun);
    }

    public Coroutine StartCoroutine(IEnumerator routine)
    {
        return controller.StartCoroutine(routine);
    }
    public Coroutine StartCoroutine(string methoName, [DefaultValue("null")] object  value)
    {
        return controller.StartCoroutine(methoName, value);
    }

    public void StopCoroutine(IEnumerator routine)
    {
        controller.StopCoroutine(routine);
    }
}
