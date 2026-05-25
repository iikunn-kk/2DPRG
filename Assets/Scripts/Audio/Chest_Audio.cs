using UnityEngine;

/// <summary>
/// 宝箱音效控制
/// 当宝箱标签变为 "Untagged" 时播放开箱音效（仅播放一次）
/// </summary>
public class Chest_Audio : MonoBehaviour
{
    private AudioSource _audioSource;
    private bool _hasPlayed;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // 防止重复播放：已播放过的宝箱不再触发
        if (_hasPlayed) return;

        if (_audioSource != null && gameObject.CompareTag("Untagged"))
        {
            _audioSource.Play();
            _hasPlayed = true;
        }
    }
}
