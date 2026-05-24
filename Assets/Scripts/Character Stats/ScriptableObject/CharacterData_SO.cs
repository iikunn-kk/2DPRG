using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Data", menuName = "Character Stats/Data")]
public class CharacterData_SO : ScriptableObject
{
    [Header("血量属性")]
    public int maxHealth;
    public int currentHealth;
    [Header("攻击属性")]
    public float minDamage;
    public float maxDamage;
    public float criticalMultiplier;//暴击增幅百分比
    public float cirticalChance;//暴击率
    [Header("滑铲能量条")]
    public float maxPower;//最大能量
    public float currentPower;//当前能量
    [Header("击杀后掉落的经验值")]
    public int killPoint;//击杀经验

    [Header("等级属性")]
    public int currentLevel;//当前等级
    public int maxLevel;//最大等级
    public int baseExp;//升级要求的经验值
    public int currentExp;//当前已获得的总经验值
    public float levelBuff;//升一级属性提升的百分比

    public float LevelMultiplier
    {
        get { return 1 + (currentLevel - 1) * levelBuff; }
    }
    public void UpdateExp(int point)
    {
        currentExp += point;
        if (currentExp >= baseExp)
            LeveUp();
    }
    private void LeveUp()//升级属性加成
    {
        //所有你想提升数据的方法
        currentLevel = Mathf.Clamp(currentLevel + 1, 0, maxLevel);
        baseExp += (int)(baseExp * LevelMultiplier);

        maxHealth = (int)(maxHealth * LevelMultiplier);
        currentHealth = maxHealth;
        minDamage += 1;
        maxDamage += 1;
        maxPower += 2.5f;
        currentPower = maxPower;
        Debug.Log("LEVEL UP!" + currentHealth + "Max Health:" + maxHealth);
    }
}
