using System.Collections;
using UnityEngine;

/// <summary>
/// 暂停面板管理器
/// </summary>
public class PausePanel : Singleton<PausePanel>
{
    public GameObject pausePanel;//游戏暂停面板
    private GameObject bgm;
    private AudioSource bgmSource;
    private PlayerAudio playerAudio;


    protected override void Awake()
    {
        base.Awake();
        // pausePanel = GameObject.Find("PausePanel");
        // pausePanel = FindFirstObjectByType<PausePanel>().gameObject;
        bgm = GameObject.Find("BGM");

        if (bgm == null)
        {
            Debug.LogWarning("[PausePanel] 未找到 BGM 对象");
        }
    }
    private void Start()
    {
        if (bgm != null)
        {
            bgmSource = bgm.GetComponent<AudioSource>();
        }

        if (bgmSource == null)
        {
            Debug.LogWarning("[PausePanel] 未找到 BGM AudioSource");
        }
    }
    private void Update()
    {
        if (playerAudio == null)
        {
            playerAudio = FindFirstObjectByType<PlayerAudio>();
        }
    }
    public void PauseGame()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        Time.timeScale = 0;

        if (bgmSource != null)
        {
            bgmSource.Pause();//暂停场景的BGM音乐
        }

        if (playerAudio != null && playerAudio.audioSource2 != null)
        {
            playerAudio.audioSource2.mute = true;//将人物走动声静音
        }
    }
    public void ContinueGame()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        Time.timeScale = 1;

        if (bgmSource != null)
        {
            bgmSource.Play();//播放场景的BGM音乐
        }

        if (playerAudio != null && playerAudio.audioSource2 != null)
        {
            playerAudio.audioSource2.mute = false;//关闭人物走动声静音
        }
    }
    public void RestartGame()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        Time.timeScale = 1;

        if (bgmSource != null)
        {
            bgmSource.Play();//播放场景的BGM音乐
        }

        if (playerAudio != null && playerAudio.audioSource2 != null)
        {
            playerAudio.audioSource2.mute = false;//关闭人物走动声静音
        }

        if (SceneController.Instance != null)
        {
            SceneController.Instance.RestartGameScene();
        }
    }
    public void QuitGame()
    {
        if (SceneController.Instance != null)
        {
            SceneController.Instance.TransitionToMain();//返回主场景
        }
        Time.timeScale = 1;
    }
    public void DeadToRestartGame()
    {
        StartCoroutine(DeadToPauseGame());
    }

    IEnumerator DeadToPauseGame()
    {
        yield return new WaitForSeconds(1f);
        PauseGame();
    }
}
