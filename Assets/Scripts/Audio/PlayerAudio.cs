using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public void PlayWithJump()
    {
        audioSource1.clip = jumping;
        audioSource1.Play();
    }

    /// <summary>
    /// 根据当前场景自动选择跑步音效（无参数，可直接绑定 UnityEvent）。
    /// 场景名含 Snow/Ice → 雪地 | Lava/Volcano → 火山 | Desert/Rocky/Mount → 岩石 | 其余默认森林
    /// </summary>
    public void PlayRunningSound()
    {
        string sceneName = SceneManager.GetActiveScene().name.ToLowerInvariant();

        AudioClip clipToPlay = running;
        if (sceneName.Contains("snow") || sceneName.Contains("ice"))
            clipToPlay = snowRunning;
        else if (sceneName.Contains("lava") || sceneName.Contains("volcano"))
            clipToPlay = volcanoRunning;
        else if (sceneName.Contains("desert") || sceneName.Contains("rocky") || sceneName.Contains("mount"))
            clipToPlay = rockyTerrainRunning;
        // else: 默认森林/草地音效 (grassRunning 与 running 相同)

        audioSource2.clip = clipToPlay;
        audioSource2.Play();
    }
}