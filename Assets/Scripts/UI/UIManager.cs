using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// UI 管理器 - 输入控制与 UI 面板协调
/// 监听快捷键：存档/读档/暂停/属性面板
/// </summary>
public class UIManager : Singleton<UIManager>
{
    private PlayerInputController _inputControl;

    [Header("UI组件")]
    public PlayerStatBar playerStatBar;

    [Header("事件监听")]
    public CharacterEventSO healthEvent;
    public CharacterEventSO updateHealthBar;

    private int _openCount;

    protected override void Awake()
    {
        base.Awake();
        _inputControl = new PlayerInputController();

        _inputControl.Gameplay.Save.started += OnSaveGame;
        _inputControl.Gameplay.Load.started += OnLoadGame;
        _inputControl.Gameplay.PausePanel.started += OnPausePanel;
        _inputControl.Gameplay.PorpertiesPanel.started += OnPropertiesPanel;
    }

    private void OnEnable()
    {
        if (healthEvent != null)
            healthEvent.OnEventRaised += OnHealthEvent;

        _inputControl?.Enable();
    }

    private void OnDisable()
    {
        if (healthEvent != null)
            healthEvent.OnEventRaised -= OnHealthEvent;

        _inputControl?.Disable();
    }

    /// <summary>
    /// 角色生命值/受伤/升级时的 UI 更新
    /// </summary>
    private void OnHealthEvent(Character character)
    {
        if (character == null) return;

        float percentage = character.maxHealth > 0
            ? (float)character.currentHealth / character.maxHealth
            : 0f;

        playerStatBar?.OnHealthChange(percentage);
        playerStatBar?.OnPowerChange(character);
    }

    private void OnSaveGame(InputAction.CallbackContext context)
    {
        SaveManager.Instance?.SavePlayerData();
    }

    private void OnLoadGame(InputAction.CallbackContext context)
    {
        SaveManager.Instance?.LoadPlayerData();
    }

    private void OnPausePanel(InputAction.CallbackContext context)
    {
        PausePanel.Instance?.PauseGame();
    }

    private void OnPropertiesPanel(InputAction.CallbackContext context)
    {
        _openCount++;
        if (_openCount == 1)
        {
            PlayerPropertiesPanel.Instance?.PropertiesPanel();
        }
        else
        {
            _openCount = 0;
            PlayerPropertiesPanel.Instance?.ClosePropertiesPanel();
        }
    }
}
