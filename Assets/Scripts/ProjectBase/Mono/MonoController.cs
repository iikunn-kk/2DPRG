using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// MonoController - 全局 Update 事件分发器
/// 挂载在不销毁的 GameObject 上，为其他模块提供帧更新回调
/// </summary>
public class MonoController : MonoBehaviour
{
    public event UnityAction updateEvent;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        updateEvent?.Invoke();
    }

    public void AddUpdateListener(UnityAction fun)
    {
        updateEvent += fun;
    }

    public void RemoveUpdateListener(UnityAction fun)
    {
        updateEvent -= fun;
    }
}
