using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesAudio : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip hurtAudio;
    public AudioClip deadAudio;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
}
