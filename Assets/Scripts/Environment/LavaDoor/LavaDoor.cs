using UnityEngine;

/// <summary>
/// 熔岩门控制器
/// 当玩家靠近时自动打开熔岩门
/// </summary>
public class LavaDoor : MonoBehaviour
{
    [Header("组件引用")]
    [SerializeField] private AudioSource audioSource; // 门开启音效
    [SerializeField] private Animator anim;           // 门动画控制器

    [Header("配置")]
    [SerializeField] private AudioClip doorOpenClip;   // 门的打开音效（可选）

    private void Awake()
    {
        // 自动获取组件（如果未手动拖入）
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        
        if (anim == null)
            anim = GetComponent<Animator>();

        // 如果有独立音效资源，使用它
        if (doorOpenClip != null && audioSource != null)
        {
            audioSource.clip = doorOpenClip;
        }
    }

    /// <summary>
    /// 播放熔岩门打开音效（可由动画事件调用）
    /// </summary>
    public void PlayLavaDoorOpenAudio()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }

    /// <summary>
    /// 玩家触发碰撞 → 开启门
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OpenDoor();
        }
    }

    /// <summary>
    /// 开启门的动画和音效
    /// </summary>
    private void OpenDoor()
    {
        if (anim != null)
        {
            anim.Play("Open");
        }
        
        PlayLavaDoorOpenAudio();
        
        Debug.Log($"[LavaDoor] 熔岩门已开启: {gameObject.name}");
    }
}
