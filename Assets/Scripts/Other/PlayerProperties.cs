using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProperties : MonoBehaviour
{

    public Character character;
    public Text levelText;
    public Text maxHpText;
    public Text currentHpText;
    public Text maxPowerText;
    public Text currentPowerText;
    public Text maxDamageText;
    public Text minDamageText;
    public Text criticalMultiplierText;
    public Text criticalChanceText;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (character == null)
        {
            character = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();
            Debug.Log("寻找中");
        }
        UpdateCurrentLevelText();
        UpdateMaxHpText();
        UpdateCurrentHpText();
        UpdateMaxPowerText();
        UpdateCurrentPowerText();
        UpdateMinDamageText();
        UpdateMaxDamageText();
        UpdateCriticalMultiplierText();
        UpdateCriticalChanceText();
    }
    void UpdateCurrentLevelText()
    {
        levelText.text = "等级: " + GameManager.Instance.characterStats.CurrentLevel.ToString("00");
    }
    void UpdateMaxHpText()
    {
        maxHpText.text = GameManager.Instance.characterStats.MaxHealth.ToString();
    }
    void UpdateCurrentHpText()
    {
        // 设置等级文本，格式为 "Level  " 加上两位数的当前等级
        currentHpText.text = GameManager.Instance.characterStats.CurrentHealth.ToString();
    }
    void UpdateMaxPowerText()
    {
        maxPowerText.text = GameManager.Instance.characterStats.MaxPower.ToString();
    }
    void UpdateCurrentPowerText()
    {
        currentPowerText.text = ((int)character.currentPower).ToString();
    }
    void UpdateMaxDamageText()
    {
        maxDamageText.text = GameManager.Instance.characterStats.MaxDamage.ToString();
    }
    void UpdateMinDamageText()
    {
        minDamageText.text = GameManager.Instance.characterStats.MinDamage.ToString();
    }
    void UpdateCriticalMultiplierText()
    {
        criticalMultiplierText.text = GameManager.Instance.characterStats.CriticalMultiplier.ToString();
    }
    void UpdateCriticalChanceText()
    {
        criticalChanceText.text = GameManager.Instance.characterStats.CriticalChance.ToString();
    }

}
