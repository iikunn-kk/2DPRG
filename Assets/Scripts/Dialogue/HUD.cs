using UnityEngine;

/// <summary>
/// HUD (Heads-Up Display) 管理器 - 单例模式
/// 管理对话UI、交互提示UI的激活状态
/// </summary>
public class HUD : Singleton<HUD>
{
    [Header("UI面板")]
    public GameObject talkUI;
    public GameObject interactableUI;
    public GameObject cubeUI;
    public GameObject pickupUI;

    [Header("输入控制")]
    public PlayerInputController playerInputController;

    protected override void Awake()
    {
        base.Awake();
        playerInputController = new PlayerInputController();
        playerInputController.Enable();
    }
}
