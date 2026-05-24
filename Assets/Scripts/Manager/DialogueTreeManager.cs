using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTreeManager : Singleton<DialogueTreeManager>
{
    public GameObject player;
    public PlayerController playerController;
    public Animator anim;
    public Rigidbody2D rb;// 刚体组件
    public float maxHealth;

    [Header("对话位置（在 Inspector 中配置）")]
    public Vector3 upPosition;
    public Vector3 upToTopPosition;
    public Vector3 upToVolcanoPosition;

    // public PlayerController player;
    protected override void Awake()
    {
        base.Awake();
    }
    void Update()
    {
        // 只在 player 为 null 时查找并缓存引用，避免每帧重复查找
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
    public void OndialogueStopMove()
    {
        Debug.Log("对话中，暂停移动");
        playerController.isDialogue = true;
    }
    public void OndialogueRecoverMove()
    {
        Debug.Log("对话结束，恢复移动");
        playerController.isDialogue = false;
    }
    public void PlayDialogueWithAttack1()
    {
        anim.Play("Attack1");
    }
    public void PlayDialogueWithAttack2()
    {
        anim.Play("Attack2");
    }
    public void PlayDialogueWithJump()
    {
        anim.Play("Jump");
    }
    public void PlayDialogueWithHurt()
    {
        anim.Play("Hurt");
    }
    public void PlayDialogueWithLand()
    {
        anim.Play("Land");
    }
    public void MakePlayerJump()
    {
        rb.AddForce(player.transform.up * 25, ForceMode2D.Impulse);
    }

    public void SetPlayerPositionUp()
    {
        player.transform.position = upPosition;
    }
    public void SetPlayerPositionUpToTop()
    {
        player.transform.position = upToTopPosition;
    }
    public void SetPlayerPositionUpToVolcano()
    {
        player.transform.position = upToVolcanoPosition;
    }

    public void CharacterCurrentMaxHealth()
    {
        maxHealth = player.GetComponent<CharacterStats>().characterData.maxHealth;
    }
    // public void TransitionToNextScene()
    // {
    //     StartCoroutine(SceneController.Instance.Transition("Mounts", TransitionDestination.DestinationTag.A));
    // }
}

