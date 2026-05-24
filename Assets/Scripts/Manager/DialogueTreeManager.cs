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
    // public PlayerController player;
    protected override void Awake()
    {
        base.Awake();
    }
    void Update()
    {
        // player = GameObject.Find("Player(Clone)");
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Debug.Log("找到了");
            playerController = player.GetComponent<PlayerController>();
            anim = player.GetComponent<Animator>();
            rb = player.GetComponent<Rigidbody2D>();
            CharacterCurrentMaxHealth();
        }
        else
        {
            // Debug.Log("未找到Player");
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
        player.transform.position = new Vector3(750, 31, 0);
    }
    public void SetPlayerPositionUpToTop()
    {
        player.transform.position = new Vector3(744, 69.5f, 0);
    }
    public void SetPlayerPositionUpToVolcano()
    {
        player.transform.position = new Vector3(11.9f, -1.4f, 0);
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

