using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 资源管理器 - 单例模式
/// 提供 Resources 目录中资源的同步/异步加载
/// </summary>
public class ResMgr : BaseManager<ResMgr>
{
    /// <summary>
    /// 同步加载资源
    /// </summary>
    public T Load<T>(string name) where T : Object
    {
        var res = Resources.Load<T>(name);

        if (res is GameObject)
            return Object.Instantiate(res);

        return res;
    }

    /// <summary>
    /// 异步加载资源
    /// </summary>
    public void LoadAsync<T>(string name, UnityAction<T> callback) where T : Object
    {
        MonoMgr.Instance.StartCoroutine(ReallyLoadAsync(name, callback));
    }

    private IEnumerator ReallyLoadAsync<T>(string name, UnityAction<T> callback) where T : Object
    {
        var r = Resources.LoadAsync<T>(name);
        yield return r;

        if (r.asset is GameObject)
            callback(Object.Instantiate(r.asset) as T);
        else
            callback(r.asset as T);
    }
}
