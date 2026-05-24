using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : Singleton<HUD>
{
    public GameObject talkUI;
    public GameObject interactableUI;
    public GameObject cubeUI;
    public GameObject pickupUI;
    public PlayerInputController playerInputController;
    // private void Awake()
    // {
    //     playerInputController = new PlayerInputController();
    //     playerInputController.Enable();
    // }

    protected override void Awake()
    {
        base.Awake();
        // DontDestroyOnLoad(this);
        playerInputController = new PlayerInputController();
        playerInputController.Enable();
    }
}
