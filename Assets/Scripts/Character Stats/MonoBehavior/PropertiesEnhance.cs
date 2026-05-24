using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertiesEnhance : MonoBehaviour
{
    public void HpAdd10()
    {
        GameManager.Instance.characterStats.characterData.maxHealth += 10;
        GameManager.Instance.characterStats.characterData.currentHealth = GameManager.Instance.characterStats.characterData.maxHealth;
        Debug.Log("最大血量增加10");
    }
    public void PpAdd10()//能量条增加10

    {
        GameManager.Instance.characterStats.characterData.maxPower += 10;
        GameManager.Instance.characterStats.characterData.currentPower = GameManager.Instance.characterStats.characterData.maxPower;
        Debug.Log("最大能量增加10");
    }
    public void HpPpDamage()
    {
        GameManager.Instance.characterStats.characterData.maxHealth += 10;
        GameManager.Instance.characterStats.characterData.currentHealth = GameManager.Instance.characterStats.characterData.maxHealth;

        GameManager.Instance.characterStats.characterData.maxPower += 10;
        GameManager.Instance.characterStats.characterData.currentPower = GameManager.Instance.characterStats.characterData.maxPower;

        GameManager.Instance.characterStats.characterData.minDamage += 1;
        GameManager.Instance.characterStats.characterData.maxDamage += 1;
    }
    public void DamageAdd1()
    {
        GameManager.Instance.characterStats.characterData.minDamage += 1;
        GameManager.Instance.characterStats.characterData.maxDamage += 1;
    }
    public void HpReduce10()
    {
        GameManager.Instance.characterStats.characterData.maxHealth -= 10;
        GameManager.Instance.characterStats.characterData.currentHealth = GameManager.Instance.characterStats.characterData.maxHealth;
    }
    public void PpReduce10()//能量条增加10

    {
        GameManager.Instance.characterStats.characterData.maxPower -= 10;
        GameManager.Instance.characterStats.characterData.currentPower = GameManager.Instance.characterStats.characterData.maxPower;
    }
    public void DamageReduce1()
    {
        GameManager.Instance.characterStats.characterData.minDamage -= 1;
        GameManager.Instance.characterStats.characterData.maxDamage -= 1;
    }
}
