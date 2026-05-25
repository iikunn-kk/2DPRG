using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddBloodEffect : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip addBloodClip;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = addBloodClip;
        audioSource.Play();
    }

    void Start()
    {
        Destroy(gameObject, 0.85f);
    }
}
