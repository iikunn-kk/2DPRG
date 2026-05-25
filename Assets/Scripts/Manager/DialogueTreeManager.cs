using UnityEngine;

/// <summary>
/// 对话树管理器
/// 负责对话系统中的玩家控制、动画播放和位置设置
/// </summary>
public class DialogueTreeManager : Singleton<DialogueTreeManager>
{
    [Header("玩家引用")]
    public GameObject player;
    public PlayerController playerController;
    public Animator anim;
    public Rigidbody2D rb;

    [Header("玩家状态")]
    public float maxHealth;

    [Header("对话位置配置")]
    public Vector3 upPosition;
    public Vector3 upToTopPosition;
    public Vector3 upToVolcanoPosition;

    [Header("跳跃力度")]
    [SerializeField] private float dialogueJumpForce = 25f;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerController = player.GetComponent<PlayerController>();
                anim = player.GetComponent<Animator>();
                rb = player.GetComponent<Rigidbody2D>();
                CharacterCurrentMaxHealth();
            }
        }
    }

    #region 对话控制

    /// <summary>
    /// 对话开始时：锁定玩家状态 + 停止移动
    /// ⚠️ 方法名由 NodeCanvas 反射调用，不可修改！
    /// </summary>
    public void OndialogueStopMove()
    {
        if (playerController != null)
        {
            // 1. 先强制切到 Idle（停止运动/动量）
            playerController.ForceToIdle();

            // 2. 再通过状态机进入 Dialogue（锁定 isDialogue = true）
            playerController.OnDialogueStart();
        }
        else
        {
            Debug.LogWarning("[DialogueTreeManager] playerController 为空");
        }
    }

    /// <summary>
    /// 对话结束时：恢复移动 + 解锁输入
    /// ⚠️ 方法名由 NodeCanvas 反射调用，不可修改！
    /// </summary>
    public void OndialogueRecoverMove()
    {
        if (playerController != null)
        {
            // 通过状态机退出 Dialogue（解锁 isDialogue = false）
            playerController.OnDialogueEnd();
        }
        else
        {
            Debug.LogWarning("[DialogueTreeManager] playerController 为空");
        }
    }

    #endregion

    #region 动画播放

    public void PlayDialogueWithAttack1()
    {
        if (anim != null) anim.Play("Attack1");
    }

    public void PlayDialogueWithAttack2()
    {
        if (anim != null) anim.Play("Attack2");
    }

    public void PlayDialogueWithJump()
    {
        if (anim != null) anim.Play("Jump");
    }

    public void PlayDialogueWithHurt()
    {
        if (anim != null) anim.Play("Hurt");
    }

    public void PlayDialogueWithLand()
    {
        if (anim != null) anim.Play("Land");
    }

    #endregion

    #region 玩家控制

    public void MakePlayerJump()
    {
        if (rb != null && player != null)
            rb.AddForce(player.transform.up * dialogueJumpForce, ForceMode2D.Impulse);
        else
            Debug.LogWarning("[DialogueTreeManager] rb 或 player 为空");
    }

    #endregion

    #region 位置设置

    public void SetPlayerPositionUp()
    {
        if (player != null)
            player.transform.position = upPosition;
    }

    public void SetPlayerPositionUpToTop()
    {
        if (player != null)
            player.transform.position = upToTopPosition;
    }

    public void SetPlayerPositionUpToVolcano()
    {
        if (player != null)
            player.transform.position = upToVolcanoPosition;
    }

    #endregion

    #region 辅助方法

    public void CharacterCurrentMaxHealth()
    {
        if (player != null)
        {
            var characterStats = player.GetComponent<CharacterStats>();
            if (characterStats != null)
                maxHealth = characterStats.characterData.maxHealth;
        }
    }

    #endregion
}
