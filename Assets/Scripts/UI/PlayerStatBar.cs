using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 玩家状态条管理类，负责更新玩家的等级文本、经验条、生命值条等 UI 元素。
/// </summary>
public class PlayerStatBar : MonoBehaviour
{

    private Character currentCharacter;
    /// <summary>
    /// 显示玩家等级的文本组件
    /// </summary>
    public Text levelText;
    /// <summary>
    /// 显示当前生命值的图像组件
    /// </summary>
    public Image healthImage;
    /// <summary>
    /// 显示延迟生命值变化的图像组件（红色血条）
    /// </summary>
    public Image healthDelayImage;
    /// <summary>
    /// 显示能量值的图像组件
    /// </summary>
    public Image powerImage;
    /// <summary>
    /// 显示当前经验值的图像组件
    /// </summary>
    public Image expImage;

    /// <summary>
    /// 血条的变化速度
    /// </summary>
    public float changeSpeed;

    public bool isRecovering;

    /// <summary>
    /// 玩家角色的属性组件
    /// </summary>
    public CharacterStats characterStats;
    // public GameObject player;

    /// <summary>
    /// 在脚本实例被加载时调用，目前方法体为空
    /// </summary>
    void Awake()
    {
        // player = GameManager.Instance.characterStats.gameObject;
        // characterStats = player.GetComponent<CharacterStats>();
        // Debug.Log(characterStats);
    }

    /// <summary>
    /// 在第一次帧更新之前调用，初始化角色属性组件
    /// </summary>
    void Start()
    {
        characterStats = GameManager.Instance.characterStats;
    }

    /// <summary>
    /// 每帧调用一次，更新玩家状态 UI
    /// </summary>
    void Update()
    {

        // 如果玩家属性为空，则不进行后续操作
        if (GameManager.PlayerStats == null)
            return;
        // 获取玩家属性组件
        characterStats = GameManager.PlayerStats;
        // 更新UI
        if (characterStats == null)
        {
            // 若未找到玩家属性组件，打印日志并返回
            Debug.Log("找不到");
            return;
        }

        // 更新等级文本
        UpdateLevelText();
        // 更新经验条
        UpdateExp();
        // 更新生命值条
        UpdateHealth();
        //更新能量值条
        UpdatePower();
        // 如果延迟生命值图像的填充量大于当前生命值图像的填充量
        if (healthDelayImage.fillAmount > healthImage.fillAmount)
        {

            // 让红色血条根据一定的速度跟随着变化
            healthDelayImage.fillAmount -= Time.deltaTime * changeSpeed;
        }


    }

    /// <summary>

    /// 接收生命值的变更百分比并更新生命值图像的填充量
    /// </summary>

    /// <param name="persentage">生命值百分比：当前生命值 / 最大生命值</param>
    public void OnHealthChange(float persentage)
    {
        // healthImage.fillAmount = persentage;
    }
    public void OnPowerChange(Character character)
    {
        isRecovering = true;
        currentCharacter = character;
    }
    /// <summary>
    /// 更新生命值条的填充量
    /// </summary>
    void UpdateHealth()
    {
        // 计算当前生命值占最大生命值的百分比
        float sliderPercent = (float)GameManager.Instance.characterStats.characterData.currentHealth / (float)GameManager.Instance.characterStats.characterData.maxHealth;
        // 更新生命值图像的填充量
        healthImage.fillAmount = sliderPercent;
        // Debug.Log(GameManager.Instance.characterStats.characterData.currentHealth);
        // Debug.Log(GameManager.Instance.characterStats.characterData.maxHealth);


        // Debug.Log(sliderPercent);
    }

    /// <summary>
    /// 更新经验条的填充量
    /// </summary>
    void UpdateExp()
    {
        // 计算当前经验值占基础经验值的百分比
        float sliderPercent = (float)GameManager.Instance.characterStats.characterData.currentExp / (float)GameManager.Instance.characterStats.characterData.baseExp;
        // 更新经验条图像的填充量
        expImage.fillAmount = sliderPercent;
        // Debug.Log(GameManager.Instance.characterStats.characterData.currentExp);
        // Debug.Log(GameManager.Instance.characterStats.characterData.baseExp);
        // Debug.Log(sliderPercent);
    }

    /// <summary>
    /// 更新显示玩家等级的文本
    /// </summary>
    void UpdateLevelText()
    {
        // 设置等级文本，格式为 "Level  " 加上两位数的当前等级
        levelText.text = "Level  " + GameManager.Instance.characterStats.characterData.currentLevel.ToString("00");
    }
    void UpdatePower()
    {
        if (isRecovering)
        {
            float persentage = currentCharacter.currentPower / currentCharacter.maxPower;
            // Debug.Log(persentage);
            powerImage.fillAmount = persentage;
            if (persentage >= 1)
            {
                isRecovering = false;
                return;
            }
        }
    }
}
