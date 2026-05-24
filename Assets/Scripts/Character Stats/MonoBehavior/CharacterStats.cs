using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public CharacterData_SO templateData;//模版Data

    public CharacterData_SO characterData; // 角色基础数据脚本化对象
    // public AttackData_SO attackData;        // 攻击数据脚本化对象
    [HideInInspector]
    // public bool isCriticak; // 注意：此处疑似拼写错误，应为 isCritical
    #region Read from Data_SO


    public int MaxHealth
    {
        get//可读
        {
            if (characterData != null)
                return characterData.maxHealth;
            else
                return 0;
        }
        set//可写
        {
            characterData.maxHealth = value;
        }
    }
    public int CurrentHealth
    {
        get
        {
            if (characterData != null)
                return characterData.currentHealth;
            else
                return 0;
        }
        set
        {
            characterData.currentHealth = value;
        }
    }
    public float MinDamage
    {
        get
        {
            if (characterData != null)
                return characterData.minDamage;
            else
                return 0;
        }
        set
        {
            characterData.minDamage = value;
        }
    }
    public float MaxDamage
    {
        get
        {
            if (characterData != null)
                return characterData.maxDamage;
            else
                return 0;
        }
        set
        {
            characterData.maxDamage = value;
        }

    }
    public float CriticalMultiplier
    {
        get
        {
            if (characterData != null)
                return characterData.criticalMultiplier;
            else
                return 0;
        }
        set
        {
            characterData.criticalMultiplier = value;
        }
    }
    public float CriticalChance
    {
        get
        {
            if (characterData != null)
                return characterData.criticalChance;
            else
                return 0;
        }
        set
        {
            characterData.criticalChance = value;
        }
    }
    public int CurrentLevel
    {
        get
        {
            if (characterData != null)
                return characterData.currentLevel;
            else
                return 0;
        }
        set
        {
            characterData.currentLevel = value;
        }
    }
    public int MaxLevel
    {
        get
        {
            if (characterData != null)
                return characterData.maxLevel;
            else
                return 0;
        }
        set
        {
            characterData.maxLevel = value;
        }
    }
    public int BaseExp
    {
        get
        {
            if (characterData != null)
                return characterData.baseExp;
            else
                return 0;
        }
        set
        {
            characterData.baseExp = value;
        }
    }

    public int CurrentExp
    {
        get
        {
            if (characterData != null)
                return characterData.currentExp;
            else
                return 0;
        }
        set
        {
            characterData.currentExp = value;
        }
    }
    public float CurrentPower
    {
        get
        {
            if (characterData != null)
                return characterData.currentPower;
            else
                return 0;
        }
        set
        {
            characterData.currentPower = value;
        }
    }
    public float MaxPower
    {
        get
        {
            if (characterData != null)
                return characterData.maxPower;
            else
                return 0;
        }
        set
        {
            characterData.maxPower = value;
        }
    }

    #endregion

    private void Awake()
    {
        if (templateData != null)
        {
            characterData = Instantiate(templateData);
        }
    }
}
