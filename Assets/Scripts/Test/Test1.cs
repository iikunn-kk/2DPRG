using UnityEngine;

/// <summary>
/// EventCenter 测试脚本（开发调试用）
/// </summary>
public class Test1 : MonoBehaviour
{
    private void OnEnable()
    {
        EventCenter.Instance.AddEventListener<int>("HelloEventCenter", OnHelloWithValue);
        EventCenter.Instance.AddEventListener("HelloEventCenter2", OnHelloNoValue);
    }

    private void OnDisable()
    {
        EventCenter.Instance.RemoveEventListener<int>("HelloEventCenter", OnHelloWithValue);
        EventCenter.Instance.RemoveEventListener("HelloEventCenter2", OnHelloNoValue);
    }

    private void OnHelloWithValue(int value)
    {
        Debug.Log($"HelloEventCenter {value}");
    }

    private void OnHelloNoValue()
    {
        Debug.Log("HelloEventCenter2 triggered");
    }
}
