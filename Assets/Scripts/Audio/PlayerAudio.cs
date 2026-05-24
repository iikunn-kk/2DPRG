using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public AudioSource audioSource1;
    public AudioSource audioSource2;
    public AudioClip jumping;
    public AudioClip running;//默认音效
    public AudioClip grassRunning;
    public AudioClip snowRunning;
    public AudioClip volcanoRunning;
    public AudioClip rockyTerrainRunning;
    public AudioClip attacking;
    public AudioClip dead;
    public AudioClip hurt;
    public bool attackAudio;

    private void Awake()
    {
        // audioSource1 = GetComponent<AudioSource>();
    }

    public void PlayWithJump()
    {
        audioSource1.clip = jumping;
        audioSource1.Play();
    }

    // public void PlayWithRunning()
    // {
    //     if (!audioSource2.isPlaying)
    //     {
    //         audioSource2.clip = running;
    //         audioSource2.Play();
    //     }
    // }

    // public void StopPlay()
    // {
    //     if (audioSource2.isPlaying)
    //     {
    //         audioSource2.Stop();
    //     }
    // }
    public void PlayRunningSound(string mapType)
    {
        AudioClip clipToPlay = running;
        switch (mapType)
        {
            case "Forest":
                clipToPlay = grassRunning;
                break;
            case "Snow":
                clipToPlay = snowRunning;
                break;
            case "Volcano":
                clipToPlay = volcanoRunning;
                break;
            case "RockyTerrain":
                clipToPlay = rockyTerrainRunning;
                break;
        }
        audioSource2.clip = clipToPlay;
        audioSource2.Play();
    }
}