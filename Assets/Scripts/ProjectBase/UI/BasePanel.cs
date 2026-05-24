using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 面板基类
/// 找到所有自己面板下的控件对象
/// 他应该提供显示或者隐藏的行为
/// </summary>
public class BasePanel : MonoBehaviour
{
    public Dictionary<string, List<UIBehaviour>> controlDic = new Dictionary<string, List<UIBehaviour>>();


    // Start is called before the first frame update
    void Start()
    {
        FindChildrenControl<Button>();
        FindChildrenControl<Image>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 找到子对象的对应控件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private void FindChildrenControl<T>() where T:UIBehaviour
    {
        T[] controls = this.GetComponentsInChildren<T>();
        string objName;
        for (int i = 0; i < controls.Length; i++)
        {
            objName = controls[i].gameObject.name;
            if (controlDic.ContainsKey(objName))
                controlDic[objName].Add(controls[i]);
            else
                controlDic.Add(objName,new List<UIBehaviour>() { controls[i] });
        }
    }
}
