using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 角色属性面板 — 将 GameManager 中的角色数据实时显示到 Text 组件
/// </summary>
public class PlayerProperties : MonoBehaviour
{
    [Header("Text 组件")]
    [SerializeField] private Text _levelText;
    [SerializeField] private Text _maxHpText;
    [SerializeField] private Text _currentHpText;
    [SerializeField] private Text _maxPowerText;
    [SerializeField] private Text _currentPowerText;
    [SerializeField] private Text _maxDamageText;
    [SerializeField] private Text _minDamageText;
    [SerializeField] private Text _criticalMultiplierText;
    [SerializeField] private Text _criticalChanceText;

    private Character _character;

    private void Start()
    {
        FindCharacter();
    }

    private void Update()
    {
        if (_character == null)
            FindCharacter();

        if (_character == null) return; // 还没找到，跳过本帧

        var stats = GameManager.Instance?.characterStats;
        if (stats == null) return;

        UpdateCurrentLevelText(stats);
        UpdateMaxHpText(stats);
        UpdateCurrentHpText(stats);
        UpdateMaxPowerText(stats);
        UpdateCurrentPowerText();
        UpdateMinDamageText(stats);
        UpdateMaxDamageText(stats);
        UpdateCriticalMultiplierText(stats);
        UpdateCriticalChanceText(stats);
    }

    private void FindCharacter()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            _character = player.GetComponent<Character>();
    }

    private void UpdateCurrentLevelText(CharacterStats stats)
    {
        if (_levelText != null)
            _levelText.text = $"等级: {stats.CurrentLevel:00}";
    }

    private void UpdateMaxHpText(CharacterStats stats)
    {
        if (_maxHpText != null)
            _maxHpText.text = stats.MaxHealth.ToString();
    }

    private void UpdateCurrentHpText(CharacterStats stats)
    {
        if (_currentHpText != null)
            _currentHpText.text = stats.CurrentHealth.ToString();
    }

    private void UpdateMaxPowerText(CharacterStats stats)
    {
        if (_maxPowerText != null)
            _maxPowerText.text = stats.MaxPower.ToString();
    }

    private void UpdateCurrentPowerText()
    {
        if (_currentPowerText != null && _character != null)
            _currentPowerText.text = ((int)_character.currentPower).ToString();
    }

    private void UpdateMinDamageText(CharacterStats stats)
    {
        if (_minDamageText != null)
            _minDamageText.text = stats.MinDamage.ToString();
    }

    private void UpdateMaxDamageText(CharacterStats stats)
    {
        if (_maxDamageText != null)
            _maxDamageText.text = stats.MaxDamage.ToString();
    }

    private void UpdateCriticalMultiplierText(CharacterStats stats)
    {
        if (_criticalMultiplierText != null)
            _criticalMultiplierText.text = stats.CriticalMultiplier.ToString();
    }

    private void UpdateCriticalChanceText(CharacterStats stats)
    {
        if (_criticalChanceText != null)
            _criticalChanceText.text = stats.CriticalChance.ToString();
    }
}
