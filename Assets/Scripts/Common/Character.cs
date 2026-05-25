using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    [Header("基础属性")]
    public float maxHealth;          // 最大生命值
    public float currentHealth;      // 当前生命值
    public float maxPower;
    public float currentPower;
    public float powerRecoverSpeed;

    [Header("陷阱伤害设置")]
    [SerializeField] private int trapDamage = 20; // 陷阱造成的伤害值

    [Header("受伤无敌")]
    public float invulnerableDuration;// 无敌状态持续时间
    private float invulnerableCounter;// 无敌状态剩余时间（运行时计数用）
    public bool invulnerable;        // 是否处于无敌状态
    // 事件系统

    public UnityEvent<Character> OnHealthChange;
    public UnityEvent<Transform> OnTakeDamage; // 受击事件（传递攻击者坐标）
    public UnityEvent OnDie;                   // 死亡事件
    [HideInInspector]
    public CharacterStats characterStats;

    void Awake()
    {
        characterStats = GetComponent<CharacterStats>();
    }
    // 初始化基础数值
    public void Start()
    {
        if (characterStats != null)
        {
            maxHealth = characterStats.characterData.maxHealth;
            currentHealth = characterStats.characterData.currentHealth;
            maxPower = characterStats.characterData.maxPower;
            currentPower = characterStats.characterData.currentPower;
        }

        if (currentHealth == 0)
        {
            this.gameObject.SetActive(false);
        }
        OnHealthChange?.Invoke(this);
    }

    // 更新无敌状态计时
    private void Update()
    {
        if (characterStats != null && maxHealth != characterStats.MaxHealth)//如果升级了，那么就更新属性
        {
            LevelUpProperty();
        }

        UpdateLoadCharacterData();


        if (invulnerable)
        {
            invulnerableCounter -= Time.deltaTime;
            if (invulnerableCounter <= 0)
            {
                invulnerable = false; // 无敌状态结束
            }
        }

        if (currentPower < maxPower)
        {
            //Debug.Log("能量恢复中");
            currentPower += Time.deltaTime * powerRecoverSpeed;
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        //如果是陷阱标签，那么就执行接下来的逻辑
        if (other.CompareTag("BloodLossTrap"))
        {
            Debug.Log("碰到扣血陷阱了");
            LogicOfPlayerFallingIntoBloodLossTrap();
        }
        if (other.CompareTag("DeathTrap"))
        {
            Debug.Log("碰到了致死陷阱");
            LogicOfPlayerFallingIntoDeathTrap();

        }
    }
    // 核心受击逻辑
    public void TakeDamage(Attack attacker)
    {
        if (invulnerable)
            return;

        //最终伤害值
        float finalDamage = CriticalDamage(attacker);

        if (currentHealth - finalDamage > 0)
        {
            Debug.Log("受到" + attacker.name + "的伤害为" + finalDamage);
            currentHealth -= finalDamage;
            characterStats.CurrentHealth = (int)currentHealth;//同步更新数据保存档位里的目前血量
            TriggerInvulnerable();            // 触发无敌状态
            OnTakeDamage?.Invoke(attacker.transform); // 传递攻击者位置，执行受伤
        }

        else
        {
            currentHealth = 0;
            OnDie?.Invoke(); // 触发死亡事件

            if (characterStats != null)
            {
                characterStats.CurrentHealth = 0;
            }

            //被攻击者击败后，提供经验值给攻击者
            if (attacker != null && attacker.characterStats != null && characterStats != null)
            {
                attacker.characterStats.characterData.UpdateExp(characterStats.characterData.killPoint);
            }
        }
        OnHealthChange?.Invoke(this);
    }

    // 激活无敌状态（带冷却时间）
    public void TriggerInvulnerable()
    {
        if (!invulnerable)
        {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration; // 重置计时器
        }
    }

    //攻击的伤害计算
    public float CriticalDamage(Attack attacker)
    {
        float finalDamage = 0;
        //是否暴击
        bool isCritical = Random.value < attacker.characterStats.CriticalChance;
        // Debug.Log("是否暴击：" + isCritical);
        //基础伤害的计算，在最小和最大之间随机取一个伤害值
        float damage = Random.Range(attacker.characterStats.MinDamage, attacker.characterStats.MaxDamage);
        // Debug.Log("基础伤害为:" + damage);
        if (isCritical)
        {
            //暴击伤害的数值计算
            finalDamage = damage * attacker.characterStats.CriticalMultiplier;
        }
        else
        {
            finalDamage = damage;
        }
        finalDamage = (int)finalDamage;
        // Debug.Log("最终伤害值为" + finalDamage);
        return finalDamage;
    }

    public void LevelUpProperty()//升级后属性数值同步
    {
        maxHealth = characterStats.MaxHealth;
        currentHealth = characterStats.CurrentHealth;
        maxPower = characterStats.characterData.maxPower;
        currentPower = characterStats.characterData.currentPower;
    }
    public void UpdateLoadCharacterData()
    {
        if (characterStats != null)
        {
            maxHealth = characterStats.characterData.maxHealth;
            currentHealth = characterStats.characterData.currentHealth;
            // OnHealthChange?.Invoke(this);
        }
    }

    /// <summary>
    /// 人物掉入陷阱后需要执行的逻辑
    /// </summary>
    public void LogicOfPlayerFallingIntoBloodLossTrap()
    {

        // currentHealth = 0;
        // characterStats.characterData.currentHealth = 0;
                if (!invulnerable)
        {
            if (characterStats.characterData.currentHealth - trapDamage > 0)
            {
                TriggerInvulnerable();//触发人物受伤时间间隔
                characterStats.characterData.currentHealth -= trapDamage;//陷阱扣血
                currentHealth = characterStats.characterData.currentHealth;//同步人物当前血量和保存在数据中的血量保持一致

                // 添加 null 检查，防止空引用崩溃
                if (PlayerController.Instance != null)
                {
                    PlayerController.Instance.isHurt = true;//设置人物受伤状态为true，可以防止静止人物的移动，在受伤动画播放后再自行设置isHurt为false然后自行恢复移动
                }

                if (PlayerAnimation.Instance != null)
                {
                    PlayerAnimation.Instance.PlayHurt();//播放人物受伤动画，同时在受伤动画状态里会自行播放受伤音效
                }
            }
            else
            {
                characterStats.characterData.currentHealth = 0;
                currentHealth = characterStats.characterData.currentHealth;
                OnDie?.Invoke();
            }
        }
    }
    public void LogicOfPlayerFallingIntoDeathTrap()
    {
        characterStats.characterData.currentHealth = 0;
        currentHealth = characterStats.characterData.currentHealth;
        OnDie?.Invoke();
    }

    public void OnSlide(float cost)
    {
        currentPower -= cost;
        OnHealthChange?.Invoke(this);
    }
}
