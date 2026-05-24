using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class UImanager : Singleton<UImanager>
{
    private PlayerInputController inputControl;
    public PlayerStatBar playerStatBar;
    [Header("事件监听")]
    /// 角色健康值变化事件的监听对象，当角色健康值发生变化时触发相应事件。
    public CharacterEventSO healthEvent;
    public CharacterEventSO updateHealthBar;
    public float OpenNumbers;
    protected override void Awake()
    {
        base.Awake();
        // DontDestroyOnLoad(this);
        inputControl = new PlayerInputController();

        inputControl.Gameplay.Save.started += SaveGameData;//保存游戏数据

        inputControl.Gameplay.Load.started += LoadGameData;//加载游戏数据

        inputControl.Gameplay.PausePanel.started += PauseGamePanel;//打开控制面板

        inputControl.Gameplay.PorpertiesPanel.started += PropertiesPanel;//打开属性面板


    }


    private void OnEnable()
    {
        // 监听角色健康值变化事件，当事件触发时调用 OnHealthEvent 方法
        healthEvent.OnEventRaised += OnHealthEvent;
        // updateHealthBar.OnEventRaised += UpdateHealthBar;
        if (inputControl != null)
        {
            inputControl.Enable();
        }
    }
    private void OnDisable()
    {
        // 取消监听角色健康值变化事件
        healthEvent.OnEventRaised -= OnHealthEvent;
        // updateHealthBar.OnEventRaised -= UpdateHealthBar;
        if (inputControl != null)
        {
            inputControl.Disable();
        }
    }


    private void OnHealthEvent(Character character)//为升级，收到伤害或则死亡时的bar更新显示
    {
        // 计算角色当前健康值占最大健康值的比例
        var persentage = character.currentHealth / character.maxHealth;
        // 调用玩家状态条的方法更新健康值显示
        playerStatBar.OnHealthChange(persentage);
        playerStatBar.OnPowerChange(character);

    }
    // private void UpdateHealthBar(Character character)//升级后的bar更新显示
    // {
    //     var persentage = character.characterStats.characterData.currentHealth / character.characterStats.characterData.maxHealth;
    //     playerStatBar.OnHealthChange(persentage);

    // }
    private void SaveGameData(InputAction.CallbackContext context)
    {
        SaveManager.Instance.SavePlayerData();
        Debug.Log("保存数据");
    }
    private void LoadGameData(InputAction.CallbackContext context)
    {
        SaveManager.Instance.LoadPlayerData();
        Debug.Log("加载数据");
    }
    private void PauseGamePanel(InputAction.CallbackContext context)
    {
        PausePanel.Instance.PauseGame();
    }
    private void PropertiesPanel(InputAction.CallbackContext context)
    {
        OpenNumbers += 1;
        if (OpenNumbers == 1)
        {
            PlayerPropertiesPanel.Instance.PropertiesPanel();
            Debug.Log("打开属性面板");
        }
        else
        {
            OpenNumbers = 0;
            PlayerPropertiesPanel.Instance.ClosePropertiesPanel();
        }
    }
}
