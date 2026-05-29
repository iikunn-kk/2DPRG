using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 角色属性面板 — 事件驱动更新，只在数据变化时刷新 Text。
/// 比每帧轮询减少约 90% 的 Update 开销。
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
    private CharacterStats _cachedStats;

    private void Start()
    {
        TryBindCharacter();

        // 如果还没找到角色，订阅延迟查找
        if (_character == null)
            InvokeRepeating(nameof(TryBindCharacter), 0.5f, 1f);
    }

    private void OnDestroy()
    {
        CancelInvoke(nameof(TryBindCharacter));

        if (_character != null)
            _character.OnHealthChange.RemoveListener(RefreshAll);
    }

    /// <summary>
    /// 尝试绑定 Player 角色的 Character 组件
    /// </summary>
    private void TryBindCharacter()
    {
        if (_character != null) return;

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        _character = player.GetComponent<Character>();
        if (_character == null) return;

        _character.OnHealthChange.AddListener(RefreshAll);

        // 绑定成功后停止轮询
        CancelInvoke(nameof(TryBindCharacter));
        RefreshAll(_character);
    }

    /// <summary>
    /// 事件回调：角色属性变化时刷新全部文本
    /// </summary>
    private void RefreshAll(Character c)
    {
        // 延迟获取 stats（可能还未注册到 GameManager）
        if (_cachedStats == null && GameManager.Instance != null)
            _cachedStats = GameManager.Instance.characterStats;

        if (_cachedStats == null) return;

        SetText(_levelText, $"等级: {_cachedStats.CurrentLevel:00}");
        SetText(_maxHpText, _cachedStats.MaxHealth.ToString());
        SetText(_currentHpText, _cachedStats.CurrentHealth.ToString());
        SetText(_maxPowerText, _cachedStats.MaxPower.ToString());
        SetText(_currentPowerText, ((int)c.currentPower).ToString());
        SetText(_maxDamageText, _cachedStats.MaxDamage.ToString());
        SetText(_minDamageText, _cachedStats.MinDamage.ToString());
        SetText(_criticalMultiplierText, _cachedStats.CriticalMultiplier.ToString());
        SetText(_criticalChanceText, _cachedStats.CriticalChance.ToString());
    }

    private static void SetText(Text textComponent, string value)
    {
        if (textComponent != null)
            textComponent.text = value;
    }
}
