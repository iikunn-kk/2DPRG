using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodBottle : MonoBehaviour
{
    public CapsuleCollider2D bloodCollider;

    public GameObject addBloodEffect;


    private void Awake()
    {
        bloodCollider = GetComponent<CapsuleCollider2D>();
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            bloodCollider.enabled = false;
            // Instantiate(addBloodEffect, GameObject.FindGameObjectWithTag("Player").transform.position + new Vector3(0, 4f, 0), Quaternion.identity);
            //GetBloodBottleAudioSource.Play();
            if (GameManager.Instance.characterStats.characterData.currentHealth + 30 > 100)
            {
                GameManager.Instance.characterStats.characterData.currentHealth = 100;
            }
            else
            {
                GameManager.Instance.characterStats.characterData.currentHealth += 30;
                Debug.Log("恢复30血量");
            }

            Instantiate(addBloodEffect, GameObject.FindGameObjectWithTag("Player").transform.position + new Vector3(0, 4f, 0), Quaternion.identity);
            Destroy(gameObject, 1f);
        }
    }
}
