using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ProjectBase.Event
{
    /// <summary>
    /// 事件信息接口
    /// </summary>
    public interface IEventInfo { }

    /// <summary>
    /// 带参数的事件信息封装
    /// </summary>
    public class EventData<T> : IEventInfo
    {
        public UnityAction<T> Actions;

        public EventData(UnityAction<T> action)
        {
            Actions += action;
        }
    }

    /// <summary>
    /// 无参数的事件信息封装
    /// </summary>
    public class EventData : IEventInfo
    {
        public UnityAction Actions;

        public EventData(UnityAction action)
        {
            Actions += action;
        }
    }
}

/// <summary>
/// 事件中心 - 单例模式
/// 
/// 功能：
/// 1. Dictionary 存储事件映射
/// 2. 委托实现观察者模式
/// 3. 泛型支持参数传递
/// 4. 类型安全检查
/// </summary>
/// <example>
/// // 添加监听
/// EventCenter.Instance.AddListener("PlayerDead", OnPlayerDead);
/// 
/// // 触发事件
/// EventCenter.Instance.Trigger("PlayerDead");
/// </example>
public class EventCenter : BaseManager<EventCenter>
{
    /// <summary>
    /// 常用事件名称常量（防止拼写错误）
    /// </summary>
    public static class Events
    {
        public const string IsGround = "isGround";
        public const string PlayerDead = "PlayerDead";
        public const string PlayerHurt = "PlayerHurt";
        public const string LevelUp = "LevelUp";
        public const string SceneLoaded = "SceneLoaded";
        public const string GamePause = "GamePause";
        public const string GameResume = "GameResume";
    }

    // key: 事件名称, value: 对应的事件委托
    private readonly Dictionary<string, ProjectBase.Event.IEventInfo> _eventDic = new Dictionary<string, ProjectBase.Event.IEventInfo>();

    #region 事件添加

    /// <summary>
    /// 添加带参数的事件监听
    /// </summary>
    /// <param name="name">事件名称</param>
    /// <param name="action">回调函数</param>
    public void AddEventListener<T>(string name, UnityAction<T> action)
    {
        if (action == null)
        {
            Debug.LogWarning($"[EventCenter] 尝试添加空监听器，事件: {name}");
            return;
        }

        if (_eventDic.TryGetValue(name, out var existingEvent))
        {
            if (existingEvent is ProjectBase.Event.EventData<T> typedEvent)
            {
                typedEvent.Actions += action;
            }
            else
            {
                Debug.LogError($"[EventCenter] 事件 '{name}' 的类型不匹配，期望类型与现有类型不同");
            }
        }
        else
        {
            _eventDic[name] = new ProjectBase.Event.EventData<T>(action);
        }
    }

    /// <summary>
    /// 添加无参数的事件监听
    /// </summary>
    /// <param name="name">事件名称</param>
    /// <param name="action">回调函数</param>
    public void AddEventListener(string name, UnityAction action)
    {
        if (action == null)
        {
            Debug.LogWarning($"[EventCenter] 尝试添加空监听器，事件: {name}");
            return;
        }

        if (_eventDic.TryGetValue(name, out var existingEvent))
        {
            if (existingEvent is ProjectBase.Event.EventData untypedEvent)
            {
                untypedEvent.Actions += action;
            }
            else
            {
                Debug.LogError($"[EventCenter] 事件 '{name}' 已存在为带参数版本");
            }
        }
        else
        {
            _eventDic[name] = new ProjectBase.Event.EventData(action);
        }
    }

    #endregion

    #region 事件移除

    /// <summary>
    /// 移除带参数的事件监听
    /// </summary>
    public void RemoveEventListener<T>(string name, UnityAction<T> action)
    {
        if (action == null) return;

        if (_eventDic.TryGetValue(name, out var eventInfo) && eventInfo is ProjectBase.Event.EventData<T> typedEvent)
        {
            typedEvent.Actions -= action;
        }
    }

    /// <summary>
    /// 移除无参数的事件监听
    /// </summary>
    public void RemoveEventListener(string name, UnityAction action)
    {
        if (action == null) return;

        if (_eventDic.TryGetValue(name, out var eventInfo) && eventInfo is ProjectBase.Event.EventData untypedEvent)
        {
            untypedEvent.Actions -= action;
        }
    }

    #endregion

    #region 事件触发

    /// <summary>
    /// 触发带参数的事件
    /// </summary>
    /// <param name="name">事件名称</param>
    /// <param name="info">事件数据</param>
    public void Trigger<T>(string name, T info)
    {
        if (_eventDic.TryGetValue(name, out var eventInfo))
        {
            if (eventInfo is ProjectBase.Event.EventData<T> typedEvent && typedEvent.Actions != null)
            {
                typedEvent.Actions.Invoke(info);
            }
            else
            {
                Debug.LogWarning($"[EventCenter] 事件 '{name}' 未注册或类型不匹配");
            }
        }
    }

    /// <summary>
    /// 触发无参数的事件
    /// </summary>
    public void Trigger(string name)
    {
        if (_eventDic.TryGetValue(name, out var eventInfo))
        {
            if (eventInfo is ProjectBase.Event.EventData untypedEvent && untypedEvent.Actions != null)
            {
                untypedEvent.Actions.Invoke();
            }
        }
    }

    #endregion

    #region 工具方法

    /// <summary>
    /// 检查事件是否已注册
    /// </summary>
    public bool HasEvent(string name)
    {
        return _eventDic.ContainsKey(name);
    }

    /// <summary>
    /// 清空所有事件（场景切换时调用）
    /// </summary>
    public void Clear()
    {
        _eventDic.Clear();
        Debug.Log("[EventCenter] 所有事件已清空");
    }

    /// <summary>
    /// 获取当前已注册的事件数量（调试用）
    /// </summary>
    public int GetEventCount()
    {
        return _eventDic.Count;
    }

    #endregion
}
