using UnityEngine;
using Cinemachine;

/// <summary>
/// 玩家绑定相机组件
/// 在 Start 时将 Cinemachine 虚拟相机绑定到玩家
/// </summary>
public class PlayerBoundCamera : MonoBehaviour
{
    private void Start()
    {
        var cinemachine = FindObjectOfType<CinemachineVirtualCamera>();
        if (cinemachine != null)
        {
            cinemachine.Follow = transform;
            cinemachine.LookAt = transform;
        }
        else
        {
            Debug.LogWarning("[PlayerBoundCamera] 场景中未找到 CinemachineVirtualCamera");
        }
    }
}
