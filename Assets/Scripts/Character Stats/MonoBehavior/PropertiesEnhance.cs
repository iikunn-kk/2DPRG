using UnityEngine;

/// <summary>
/// 属性增强组件 - 供 UnityEvent / 升级系统调用
/// 直接修改 GameManager 中的角色属性
/// </summary>
public class PropertiesEnhance : MonoBehaviour
{
    private const int HealthDelta = 10;
    private const int PowerDelta = 10;
    private const int DamageDelta = 1;

    private CharacterStats Stats => GameManager.Instance?.characterStats;
    private CharacterData_SO Data => Stats?.characterData;

    public void HpAdd10()
    {
        if (Data == null) return;
        Data.maxHealth += HealthDelta;
        Data.currentHealth = Data.maxHealth;
    }

    public void PpAdd10()
    {
        if (Data == null) return;
        Data.maxPower += PowerDelta;
        Data.currentPower = Data.maxPower;
    }

    public void HpPpDamage()
    {
        if (Data == null) return;
        Data.maxHealth += HealthDelta;
        Data.currentHealth = Data.maxHealth;
        Data.maxPower += PowerDelta;
        Data.currentPower = Data.maxPower;
        Data.minDamage += DamageDelta;
        Data.maxDamage += DamageDelta;
    }

    public void DamageAdd1()
    {
        if (Data == null) return;
        Data.minDamage += DamageDelta;
        Data.maxDamage += DamageDelta;
    }

    public void HpReduce10()
    {
        if (Data == null) return;
        Data.maxHealth -= HealthDelta;
        Data.currentHealth = Data.maxHealth;
    }

    public void PpReduce10()
    {
        if (Data == null) return;
        Data.maxPower -= PowerDelta;
        Data.currentPower = Data.maxPower;
    }

    public void DamageReduce1()
    {
        if (Data == null) return;
        Data.minDamage -= DamageDelta;
        Data.maxDamage -= DamageDelta;
    }
}
