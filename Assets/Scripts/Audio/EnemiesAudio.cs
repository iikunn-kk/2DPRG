using UnityEngine;

/// <summary>
/// 敌人音频管理器
/// 统一管理敌人的受伤和死亡音效
/// </summary>
public class EnemiesAudio : MonoBehaviour
{
    [Header("音频源")]
    public AudioSource audioSource; // 音频播放器

    [Header("音效资源")]
    public AudioClip hurtAudio;  // 受伤音效
    public AudioClip deadAudio;   // 死亡音效

    private void Awake()
    {
        // 自动获取 AudioSource（如果未手动拖入）
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            
            if (audioSource == null)
            {
                Debug.LogWarning($"[EnemiesAudio] 未找到 AudioSource: {gameObject.name}");
            }
        }
    }

    /// <summary>
    /// 播放受伤音效
    /// </summary>
    public void PlayHurtSound()
    {
        if (audioSource != null && hurtAudio != null)
        {
            audioSource.clip = hurtAudio;
            audioSource.Play();
        }
    }

    /// <summary>
    /// 播放死亡音效
    /// </summary>
    public void PlayDeadSound()
    {
        if (audioSource != null && deadAudio != null)
        {
            audioSource.clip = deadAudio;
            audioSource.Play();
        }
    }
}
