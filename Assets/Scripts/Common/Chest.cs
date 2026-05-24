using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    private SpriteRenderer spriteRenderer;
    public Sprite openSprite;
    public Sprite closeSprite;
    public bool isDone;
    public GameObject bloodBottle;//掉落血瓶

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnEnable()
    {
        spriteRenderer.sprite = isDone ? openSprite : closeSprite;
    }
    public void TriggerAction()
    {
        Debug.Log("Open Chest");
        if (!isDone)
        {
            OpenChest();
        }
    }

    private void OpenChest()
    {
        spriteRenderer.sprite = openSprite;
        isDone = true;
        gameObject.tag = "Untagged";
        Instantiate(bloodBottle, transform.position + new Vector3(0, 3, 0), Quaternion.identity);
    }
}
