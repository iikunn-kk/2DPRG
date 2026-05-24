using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class PlayerBoundCamera : MonoBehaviour
{
    [System.Obsolete]
    void Start()
    {
        // 绑定相机
        // var cinemachine = GetComponent<CinemachineVirtualCamera>();
        var cinemachine = FindObjectOfType<CinemachineVirtualCamera>();
        if (cinemachine != null)
        {
            // cinemachine.Follow = GameManager.Instance.characterStats.transform;
            // cinemachine.LookAt = GameManager.Instance.characterStats.transform;
            cinemachine.Follow = this.transform;
            cinemachine.LookAt = this.transform;
            Debug.Log("虚拟相机定位成功");
        }
        else
        {
            Debug.Log("虚拟相机定位失败");
        }
    }
}
