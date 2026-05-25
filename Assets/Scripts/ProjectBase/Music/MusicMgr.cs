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
                Object.Destroy(_soundList[i]);
                _soundList.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    public void PlayBkMusic(string name)
    {
        if (_bkMusic == null)
        {
            var obj = new GameObject("BkMusic");
            _bkMusic = obj.AddComponent<AudioSource>();
        }

        ResMgr.Instance.LoadAsync<AudioClip>("Music/BK/" + name, (clip) =>
        {
            _bkMusic.clip = clip;
            _bkMusic.loop = true;
            _bkMusic.volume = _bkVolume;
            _bkMusic.Play();
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
        {
            _soundObj = new GameObject("Sound");
        }

        ResMgr.Instance.LoadAsync<AudioClip>("Music/Sound/" + name, (clip) =>
        {
            var source = _soundObj.AddComponent<AudioSource>();
            source.clip = clip;
            source.loop = isLoop;
            source.volume = _soundVolume;
            source.Play();
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
            source.Stop();
            Object.Destroy(source);
        }
    }
}
