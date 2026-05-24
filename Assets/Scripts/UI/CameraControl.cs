using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public CinemachineImpulseSource impulseSource;
    public VoidEventSO cameraShakeEvent;

    void OnEnable()
    {
        cameraShakeEvent.OnEventRaised += OnCameraShakeEvent;
    }
    void OnDisable()
    {
        cameraShakeEvent.OnEventRaised -= OnCameraShakeEvent;
    }
    private void OnCameraShakeEvent()
    {
        impulseSource.GenerateImpulse();
    }

}
