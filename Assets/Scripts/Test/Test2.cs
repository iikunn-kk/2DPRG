using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// EventCenter 测试脚本（开发调试用）
/// </summary>
public class Test2 : MonoBehaviour
{
    public Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            Debug.Log("Click Button");
            
            // 测试带参数事件触发
            EventCenter.Instance.Trigger<int>("HelloEventCenter", 100);
            
            // 测试无参数事件触发
            EventCenter.Instance.Trigger("HelloEventCenter2");
        });
    }

    void Update()
    {
        // 预留扩展
    }
}
