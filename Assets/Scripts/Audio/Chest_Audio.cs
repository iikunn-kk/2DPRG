using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest_Audio : MonoBehaviour
{
    private AudioSource audioSource;
    public bool isOpen;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if (!isOpen && gameObject.CompareTag("Untagged"))
        {
            audioSource.Play();
            isOpen = true;
        }
    }
}
