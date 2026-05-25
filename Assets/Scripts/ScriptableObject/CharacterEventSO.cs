using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 角色事件 ScriptableObject
/// 用于在角色之间传递事件数据（受伤/死亡/升级等）
/// </summary>
[CreateAssetMenu(menuName = "Event/CharacterEventSO")]
public class CharacterEventSO : ScriptableObject
{
    public UnityAction<Character> OnEventRaised;

    public void RaiseEvent(Character character)
    {
        OnEventRaised?.Invoke(character);
    }
}
