using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MusicMgr : BaseManager<MusicMgr>
{
    [SerializeField]public  AudioSource bkMusic =  null;
    private float bkValue = 1f;

    private GameObject soundObj = null;
    private List<AudioSource> soundList = new List<AudioSource>();
    private float soundValue = 1f;

    //构造函数
    public MusicMgr()
    {
        MonoMgr.Instance.AddUpdateListener(Update);
    }
    private void Update()
    {
        for(int i = soundList.Count - 1;i>=0;i--)
        {
            if (!soundList[i].isPlaying)
            {
                GameObject.Destroy(soundList[i]);
                soundList.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="name"></param>
    public void PlayBkMusic(string name)
    {
        if(bkMusic == null)
        {
            GameObject obj = new GameObject();
            obj.name = "BkMusic";
            bkMusic = obj.AddComponent<AudioSource>();
        }

        ResMgr.Instance.LoadAsync<AudioClip>("Music/BK/"+name,(clip)=>
        {
            bkMusic.clip = clip;
            bkMusic.loop = true;
            bkMusic.volume = bkValue;
            bkMusic.Play();
        });
    }


    /// <summary>
    /// 暂停背景音乐
    /// </summary>
    public void PauseBKMusic()
    {
        if(bkMusic == null)
        {
            return;
        }
        bkMusic.Pause();
    }

    /// <summary>
    /// 从暂停的位置开始播放
    /// </summary>
    public void UnPauseBKMusic()
    {
        if(bkMusic ==null)
        {
            return;
        }
        bkMusic.UnPause();
    }
    /// <summary>
    /// 停止背景音乐
    /// </summary>
    public void StopBkMusic()
    {
        if (bkMusic == null)
        {
            return;
        }
        bkMusic.Stop();
    }

    /// <summary>
    /// 改变背景音乐 音量大小
    /// </summary>
    /// <param name="v"></param>
    public void ChangeBKValue(float v)
    {
        bkValue = v;
        if (bkMusic == null)
        {
            return;
        }
        bkMusic.volume = bkValue;
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="name"></param>
    public void PlaySound(string name,bool isLoop, UnityAction<AudioSource> callback = null)
    {
        if(soundObj == null)
        {
            soundObj = new GameObject();
            soundObj.name = "Sound";
        }  
        //当音效资源异步加载结束后，再添加音效
        ResMgr.Instance.LoadAsync<AudioClip>("Music/Sound/" + name, (clip) =>
        {
            AudioSource source = soundObj.AddComponent<AudioSource>();
            source.clip = clip;
            source.loop = isLoop;
            source.volume = soundValue;
            source.Play();
            soundList.Add(source);
            if(callback != null)
            {
                callback(source);
            }
        });


    }

    /// <summary>
    /// 改变音效音量大小
    /// </summary>
    /// <param name="value"></param>
    public void ChangeSoundValue(float value)
    {
        soundValue = value;
        for(int i = 0;i<soundList.Count;i++)
        {
            soundList[i].volume = soundValue;
        }
    }
    /// <summary>
    /// 停止音效
    /// </summary>
    public void StopSound(AudioSource source)
    {
        if(soundList.Contains(source))
        {
            soundList.Remove(source);
            source.Stop();
            GameObject.Destroy(source);
        }
    }


}
