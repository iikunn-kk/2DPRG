using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 玩家状态条管理器
/// 负责更新玩家的等级、经验值、生命值、能量值等 UI 元素
/// </summary>
public class PlayerStatBar : MonoBehaviour
{
    [Header("UI 组件引用")]
    public Text levelText;              // 等级文本显示
    public Image healthImage;           // 当前生命值条
    public Image healthDelayImage;      // 延迟生命值条（红色血条）
    public Image powerImage;            // 能量值条
    public Image expImage;              // 经验值条

    [Header("配置")]
    [SerializeField] private float changeSpeed = 2f; // 血条变化速度

    // 运行时状态
    private CharacterStats _characterStats;
    private Character _currentCharacter;
    private bool _isRecovering;

    void Start()
    {
        InitializeReferences();
    }

    /// <summary>
    /// 初始化组件引用
    /// </summary>
    private void InitializeReferences()
    {
        if (GameManager.Instance != null && GameManager.Instance.characterStats != null)
        {
            _characterStats = GameManager.Instance.characterStats;
        }
        else
        {
            Debug.LogWarning("[PlayerStatBar] GameManager 或 characterStats 未初始化");
        }
    }

    /// <summary>
    /// 每帧更新 UI 显示
    /// </summary>
    void Update()
    {
        if (_characterStats == null)
        {
            InitializeReferences(); // 尝试重新初始化
            if (_characterStats == null) return;
        }

        UpdateLevelText();
        UpdateExp();
        UpdateHealth();
        UpdatePower();
        UpdateHealthDelay();
    }

    #region UI 更新方法

    /// <summary>
    /// 更新等级文本显示
    /// </summary>
    private void UpdateLevelText()
    {
        if (levelText != null && _characterStats != null)
        {
            levelText.text = "Level  " + _characterStats.characterData.currentLevel.ToString("00");
        }
    }

    /// <summary>
    /// 更新经验值条填充量
    /// </summary>
    private void UpdateExp()
    {
        if (expImage == null || _characterStats?.characterData == null) return;

        float sliderPercent = (float)_characterStats.characterData.currentExp / 
                             (float)_characterStats.characterData.baseExp;
        expImage.fillAmount = Mathf.Clamp01(sliderPercent);
    }

    /// <summary>
    /// 更新生命值条填充量
    /// </summary>
    private void UpdateHealth()
    {
        if (healthImage == null || _characterStats?.characterData == null) return;

        float maxHealth = _characterStats.characterData.maxHealth;
        
        // 防止除以零
        if (maxHealth <= 0)
        {
            healthImage.fillAmount = 0;
            return;
        }

        float currentHealth = _characterStats.characterData.currentHealth;
        float sliderPercent = currentHealth / maxHealth;
        healthImage.fillAmount = Mathf.Clamp01(sliderPercent);
    }

    /// <summary>
    /// 更新能量值条填充量
    /// </summary>
    private void UpdatePower()
    {
        if (powerImage == null || _currentCharacter == null) return;

        if (_isRecovering)
        {
            // 防止除以零
            if (_currentCharacter.maxPower <= 0) return;

            float percentage = _currentCharacter.currentPower / _currentCharacter.maxPower;
            powerImage.fillAmount = Mathf.Clamp01(percentage);

            if (percentage >= 1f)
            {
                _isRecovering = false;
            }
        }
    }

    /// <summary>
    /// 更新延迟血条（红色血条）的平滑跟随效果
    /// </summary>
    private void UpdateHealthDelay()
    {
        if (healthDelayImage == null || healthImage == null) return;

        if (healthDelayImage.fillAmount > healthImage.fillAmount)
        {
            healthDelayImage.fillAmount -= Time.deltaTime * changeSpeed;
            
            // 防止低于当前血条
            if (healthDelayImage.fillAmount < healthImage.fillAmount)
            {
                healthDelayImage.fillAmount = healthImage.fillAmount;
            }
        }
    }

    #endregion

    #region 事件回调

    /// <summary>
    /// 接收生命值变更事件（预留接口）
    /// </summary>
    /// <param name="percentage">生命值百分比</param>
    public void OnHealthChange(float percentage)
    {
        // 当前由 Update() 直接驱动，此方法保留作为事件回调入口
    }

    /// <summary>
    /// 接收能量变更事件
    /// </summary>
    /// <param name="character">触发事件的角色</param>
    public void OnPowerChange(Character character)
    {
        _isRecovering = true;
        _currentCharacter = character;
    }

    #endregion
}
