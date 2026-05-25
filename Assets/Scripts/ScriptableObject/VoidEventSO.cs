using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 无参数事件 ScriptableObject
/// 用于全局事件广播（相机震动/场景加载等）
/// </summary>
[CreateAssetMenu(menuName = "Event/VoidEventSO")]
public class VoidEventSO : ScriptableObject
{
    public UnityAction OnEventRaised;

    public void RaiseEvent()
    {
        OnEventRaised?.Invoke();
    }
}
