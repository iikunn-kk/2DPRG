using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
// 创建Asset菜单项（在Unity编辑器中右键Create菜单可见）
[CreateAssetMenu(menuName = "Event/CharacterEventSO")]
public class CharacterEventSO : ScriptableObject
{
    // 定义带Character参数的事件委托
    public UnityAction<Character> OnEventRaised;

    // 触发事件的方法进行广播
    public void RaiseEvent(Character character)
    {
        OnEventRaised?.Invoke(character); // 调用所有订阅者的方法（添加空检查避免无订阅者时崩溃）
    }
}
