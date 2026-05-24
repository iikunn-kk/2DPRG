using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 场景退出
/// </summary> <summary>
/// 
/// </summary>
public class SceneExit : MonoBehaviour
{
    [Tooltip("需要过渡新场景的名称")]
    public string newSceneName;
    //当玩家进入触发器
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("玩家进入传送区域");
            TransitionInternal();
        }
    }
    //调用场景切换函数
    public void TransitionInternal()
    {
        SceneLoader.Instance.TransitionToScene(newSceneName);
    }
}
