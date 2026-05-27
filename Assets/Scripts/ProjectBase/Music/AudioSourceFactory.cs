using UnityEngine;

/// <summary>
/// AudioSource 工厂 — 静态工厂模式 + 对象池
/// 通过 AudioSourcePool 复用 AudioSource，避免频繁创建/销毁 GC
/// </summary>
public static class AudioSourceFactory
{
    /// <summary>
    /// 创建一个已完整配置的 AudioSource（从对象池获取）
    /// </summary>
    public static AudioSource Create(GameObject parent, string name, AudioClip clip,
                                      bool loop, float volume, bool autoPlay = true)
    {
        // 从池中获取（复用或新建）
        var source = AudioSourcePool.Get();

        // 配置
        source.gameObject.name = name;
        source.transform.SetParent(parent?.transform);
        source.clip = clip;
        source.loop = loop;
        source.volume = volume;

        if (autoPlay)
            source.Play();

        return source;
    }

    /// <summary>
    /// 归还 AudioSource 到池（播放完毕后调用）
    /// </summary>
    public static void Return(AudioSource source)
    {
        AudioSourcePool.Return(source);
    }
}
