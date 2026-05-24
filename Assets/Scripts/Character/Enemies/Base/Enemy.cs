using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator anim;
    [HideInInspector] public PhysicsCheck physicsCheck;
    [HideInInspector] public EnemiesAudio enemiesAudio;
    [Header("基本参数")]
    public float normalSpeed;
    public float chaseSpeed;
    public float currentSpeed;
    public Vector3 faceDir;
    public float hurtForce;
    public Transform attacker;

    [Header("检测前方是否有玩家的变量")]
    public Vector2 centerOffset;
    public Vector2 checkSize;
    public float checkDistance;
    public LayerMask attackLayer;
    // 增加一个变量来控制翻转后的延迟,防止原地反复发生碰撞检测
    [Header("延时检测计时器")]
    public float flipDelay = 0.2f;
    public float flipTimer = 0f;
    [Header("碰墙Idle计时器")]
    public float waitTime = 2f;          // 等待时间设定值为2秒
    public float waitTimeCounter;   // 等待时间计数器
    public bool wait;               // 是否处于等待状态

    public float lostTime;//前方丢失玩家目标时候等待一定时间就从追击状态切换回巡逻状态的等待时间
    public float lostTimeCounter;//前方丢失玩家目标时候等待一定时间就从追击状态切换回巡逻状态的切换时间计时器

    [Header("状态")]
    public bool isHurt;
    public bool isDead;
    private BaseState currentState;
    protected BaseState patrolState;
    protected BaseState chaseState;
    protected BaseState attackState;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        physicsCheck = GetComponent<PhysicsCheck>();
        enemiesAudio = GetComponent<EnemiesAudio>();
        currentSpeed = normalSpeed;
        waitTimeCounter = waitTime;
    }
    void OnEnable()
    {
        currentState = patrolState;
        currentState.OnEnter(this);
    }

    private void Update()
    {
        faceDir = new Vector3(-transform.localScale.x, 0, 0);
        // 减少翻转计时器
        if (flipTimer > 0)
        {
            flipTimer -= Time.deltaTime;
        }
        currentState.LogicUpdate();
        TimeCounter();
    }

    private void FixedUpdate()
    {
        if (!isHurt & !isDead)//按位与计算同为1才是1
            Move();

        currentState.PhysicsUpdate();
    }
    private void OnDisable()
    {
        currentState.OnExit();
    }
    public virtual void Move()
    {
        if (!wait)
        {
            rb.velocity = new Vector2(currentSpeed * faceDir.x, rb.velocity.y);
            anim.SetBool("walk", true); // 播放walk动画
        }
    }

    public void TimeCounter()
    {
        if (wait)
        {
            waitTimeCounter -= Time.deltaTime;  // 递减计时器
            if (waitTimeCounter <= 0)
            {
                wait = false;                   // 结束等待
                waitTimeCounter = waitTime;     // 重置计时器
                transform.localScale = new Vector3(faceDir.x, transform.localScale.y, transform.localScale.z); // 翻转敌人
            }
        }
        if (!FoundPlayer() && lostTimeCounter > 0)
        {
            lostTimeCounter -= Time.deltaTime;
        }
        if (FoundPlayer())
        {
            lostTimeCounter = lostTime;
        }
    }

    public bool FoundPlayer()
    {
        return Physics2D.BoxCast(transform.position + (Vector3)centerOffset, checkSize, 0, faceDir, checkDistance, attackLayer);
    }

    public void SwitchState(EnemyState state)
    {
        var newState = state switch
        {
            EnemyState.Patrol => patrolState,
            EnemyState.Chase => chaseState,
            EnemyState.Attack => attackState,
            _ => null
        };
        currentState.OnExit();
        currentState = newState;
        currentState.OnEnter(this);

    }
    #region 事件执行方法
    public void OnTakeDamage(Transform attackTrans)
    {

        attacker = attackTrans;
        //转身
        Debug.Log("发生翻转");
        if (attackTrans.position.x - transform.position.x > 0)
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        if (attackTrans.position.x - transform.position.x < 0)
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        isHurt = true;
        anim.SetTrigger("hurt");
        Vector2 dir = new Vector2(transform.position.x - attackTrans.position.x, 0).normalized;
        rb.velocity = new Vector2(0, rb.velocity.y);
        enemiesAudio.audioSource.clip = enemiesAudio.hurtAudio;
        enemiesAudio.audioSource.Play();
        Debug.Log("播放" + gameObject.name + "的受伤音效");
        StartCoroutine(OnHurt(dir));
    }
    private IEnumerator OnHurt(Vector2 dir)
    {
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.45f);
        isHurt = false;
    }


    public void OnDie()
    {
        gameObject.layer = 2;

        currentSpeed = 0;

        anim.SetBool("dead", true);
        isDead = true;

        enemiesAudio.audioSource.clip = enemiesAudio.deadAudio;
        enemiesAudio.audioSource.Play();
        Debug.Log(this.gameObject.name + "当前对象的死亡音效播放");

        StartCoroutine("DestroyObject");
    }

    protected virtual void DestroyAfterAnimation()
    {
        // Destroy(gameObject);
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + (Vector3)centerOffset + new Vector3(checkDistance * -transform.localScale.x, 0), 0.2f);
    }

    IEnumerator DestroyObject()
    {
        Debug.Log("销毁死亡的物体");
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);//销毁该物体
    }
}