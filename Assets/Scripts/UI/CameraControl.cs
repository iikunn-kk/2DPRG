using Cinemachine;
using UnityEngine;

/// <summary>
/// 相机控制 - 相机震动效果
/// 监听 VoidEventSO 事件触发 Cinemachine Impulse
/// </summary>
public class CameraControl : MonoBehaviour
{
    [SerializeField] private CinemachineImpulseSource _impulseSource;
    [SerializeField] private VoidEventSO _cameraShakeEvent;

    private void OnEnable()
    {
        if (_cameraShakeEvent != null)
            _cameraShakeEvent.OnEventRaised += OnCameraShakeEvent;
    }

    private void OnDisable()
    {
        if (_cameraShakeEvent != null)
            _cameraShakeEvent.OnEventRaised -= OnCameraShakeEvent;
    }

    private void OnCameraShakeEvent()
    {
        if (_impulseSource != null)
            _impulseSource.GenerateImpulse();
    }
}
