using UnityEngine;

/// <summary>
/// 木头人受伤反馈组件
/// 响应攻击事件播放动画和音效
/// </summary>
public class WoodManHurt : MonoBehaviour
{
    [SerializeField] private Animator _anim;
    [SerializeField] private AudioSource _audioSource;

    public void HitFromPlayer()
    {
        if (_anim != null)
            _anim.Play("Hit");

        if (_audioSource != null)
            _audioSource.Play();
    }
}
