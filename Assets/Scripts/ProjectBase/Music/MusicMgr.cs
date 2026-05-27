using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 音频管理器 - 单例模式
/// 管理背景音乐和音效的播放、暂停、音量控制
/// 自动清理播放完毕的音效 AudioSource
/// </summary>
public class MusicMgr : BaseManager<MusicMgr>
{
    [SerializeField] private AudioSource _bkMusic;
    [SerializeField] private float _bkVolume = 1f;

    private GameObject _soundObj;
    private readonly List<AudioSource> _soundList = new List<AudioSource>();
    [SerializeField] private float _soundVolume = 1f;

    public MusicMgr()
    {
        MonoMgr.Instance.AddUpdateListener(Update);
    }

    private void Update()
    {
        for (int i = _soundList.Count - 1; i >= 0; i--)
        {
            if (!_soundList[i].isPlaying)
            {
                // 归还到对象池（不再 Destroy，避免 GC）
                AudioSourceFactory.Return(_soundList[i]);
                _soundList.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    public void PlayBkMusic(string name)
    {
        ResMgr.Instance.LoadAsync<AudioClip>("Music/BK/" + name, (clip) =>
        {
            if (_bkMusic == null)
            {
                // 工厂一行搞定：new GameObject + AddComponent + 配置
                _bkMusic = AudioSourceFactory.Create(null, "BkMusic", clip, true, _bkVolume);
            }
            else
            {
                _bkMusic.clip = clip;
                _bkMusic.Play();
            }
        });
    }

    public void PauseBKMusic()
    {
        _bkMusic?.Pause();
    }

    public void UnPauseBKMusic()
    {
        _bkMusic?.UnPause();
    }

    public void StopBkMusic()
    {
        _bkMusic?.Stop();
    }

    public void ChangeBKValue(float v)
    {
        _bkVolume = v;
        if (_bkMusic != null)
            _bkMusic.volume = _bkVolume;
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    public void PlaySound(string name, bool isLoop, UnityAction<AudioSource> callback = null)
    {
        if (_soundObj == null)
            _soundObj = new GameObject("Sound");

        ResMgr.Instance.LoadAsync<AudioClip>("Music/Sound/" + name, (clip) =>
        {
            // 工厂创建：挂载到 _soundObj 下，自动配置并播放
            var source = AudioSourceFactory.Create(_soundObj, name, clip, isLoop, _soundVolume);
            _soundList.Add(source);
            callback?.Invoke(source);
        });
    }

    public void ChangeSoundValue(float value)
    {
        _soundVolume = value;
        foreach (var source in _soundList)
        {
            source.volume = _soundVolume;
        }
    }

    public void StopSound(AudioSource source)
    {
        if (_soundList.Contains(source))
        {
            _soundList.Remove(source);
            AudioSourceFactory.Return(source); // 归还池
        }
    }
}
