using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// UI 面板基类
/// 自动查找子对象中的 Button/Image 控件并缓存到字典中
/// </summary>
public class BasePanel : MonoBehaviour
{
    private readonly Dictionary<string, List<UIBehaviour>> _controlDic = new Dictionary<string, List<UIBehaviour>>();

    private void Start()
    {
        FindChildrenControl<Button>();
        FindChildrenControl<Image>();
    }

    /// <summary>
    /// 获取指定名称的控件列表
    /// </summary>
    public List<UIBehaviour> GetControls(string name)
    {
        return _controlDic.TryGetValue(name, out var list) ? list : null;
    }

    private void FindChildrenControl<T>() where T : UIBehaviour
    {
        var controls = GetComponentsInChildren<T>();
        foreach (var control in controls)
        {
            string objName = control.gameObject.name;
            if (_controlDic.ContainsKey(objName))
                _controlDic[objName].Add(control);
            else
                _controlDic.Add(objName, new List<UIBehaviour> { control });
        }
    }
}
