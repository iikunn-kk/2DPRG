using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 玩家音频管理器
/// 根据场景自动选择合适的音效（跑步、跳跃、攻击等）
/// </summary>
public class PlayerAudio : MonoBehaviour
{
    [Header("音频源")]
    public AudioSource audioSource1; // 主要音效（跳跃/攻击/死亡）
    public AudioSource audioSource2; // 环境音效（脚步声）

    [Header("音效资源 - 移动")]
    public AudioClip jumping;              // 跳跃音效
    public AudioClip running;               // 默认音效（森林）
    public AudioClip grassRunning;          // 草地音效
    public AudioClip snowRunning;           // 雪地音效
    public AudioClip volcanoRunning;        // 火山音效
    public AudioClip rockyTerrainRunning;   // 岩石地形音效

    [Header("音效资源 - 战斗")]
    public AudioClip attacking;             // 攻击音效
    public AudioClip dead;                  // 死亡音效
    public AudioClip hurt;                  // 受伤音效
    public AudioClip slideRunning;          // 滑铲音效（暂留空，后续拖入即可生效）

    [Header("运行时状态")]
    public bool attackAudio;                // 攻击音频标志（供外部查询）

    /// <summary>
    /// 播放跳跃音效
    /// </summary>
    public void PlayWithJump()
    {
        if (audioSource1 != null && jumping != null)
        {
            audioSource1.clip = jumping;
            audioSource1.Play();
        }
    }

    /// <summary>
    /// 根据当前场景自动选择跑步音效
    /// 场景命名规则：
    /// - Snow/Ice → 雪地音效
    /// - Lava/Volcano → 火山音效
    /// - Desert/Rocky/Mount → 岩石音效
    /// - 其他 → 默认森林音效
    /// </summary>
    public void PlayRunningSound()
    {
        if (audioSource2 == null)
        {
            Debug.LogWarning("[PlayerAudio] audioSource2 未设置");
            return;
        }

        string sceneName = SceneManager.GetActiveScene().name.ToLowerInvariant();
        AudioClip clipToPlay = GetRunningClipForScene(sceneName);

        if (clipToPlay != null)
        {
            audioSource2.clip = clipToPlay;
            audioSource2.Play();
        }
        else
        {
            Debug.LogWarning($"[PlayerAudio] 场景 '{sceneName}' 无对应跑步音效，使用默认音效");
            
            if (running != null)
            {
                audioSource2.clip = running;
                audioSource2.Play();
            }
        }
    }

    /// <summary>
    /// 根据场景名称获取对应的跑步音效
    /// </summary>
    /// <param name="sceneName">场景名称（小写）</param>
    /// <returns>对应的 AudioClips</returns>
    private AudioClip GetRunningClipForScene(string sceneName)
    {
        if (sceneName.Contains("snow") || sceneName.Contains("ice"))
            return snowRunning;
        
        if (sceneName.Contains("lava") || sceneName.Contains("volcano"))
            return volcanoRunning;
        
        if (sceneName.Contains("desert") || sceneName.Contains("rocky") || sceneName.Contains("mount"))
            return rockyTerrainRunning;
        
        // 默认返回草地或通用跑步音效
        return grassRunning ?? running;
    }

    /// <summary>
    /// 播放滑铲音效（仅当 Inspector 中拖入了 Clip 才播放）
    /// </summary>
    public void PlaySlideSound()
    {
        if (audioSource1 != null && slideRunning != null)
        {
            audioSource1.PlayOneShot(slideRunning);
        }
    }

    /// <summary>
    /// 播放攻击音效
    /// </summary>
    public void PlayAttackSound()
    {
        if (audioSource1 != null && attacking != null)
        {
            audioSource1.clip = attacking;
            audioSource1.Play();
        }
        attackAudio = true;
    }

    /// <summary>
    /// 播放受伤音效
    /// </summary>
    public void PlayHurtSound()
    {
        if (audioSource1 != null && hurt != null)
        {
            audioSource1.clip = hurt;
            audioSource1.Play();
        }
    }

    /// <summary>
    /// 播放死亡音效
    /// </summary>
    public void PlayDeadSound()
    {
        if (audioSource1 != null && dead != null)
        {
            audioSource1.clip = dead;
            audioSource1.Play();
        }
    }
}
