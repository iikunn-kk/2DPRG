using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodManHurt : MonoBehaviour
{
    public Animator anim;
    public AudioSource audioSource;

    public void HitFromPlayer()
    {
        anim.Play("Hit");
        audioSource.Play();
    }
}
