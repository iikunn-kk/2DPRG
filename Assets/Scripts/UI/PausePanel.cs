using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }
    private void Start()
    {
        bgmSource = bgm.GetComponent<AudioSource>();
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
        pausePanel.SetActive(true);
        Time.timeScale = 0;
        bgmSource.Pause();//暂停场景的BGM音乐
        playerAudio.audioSource2.mute = true;//将人物走动声静音
    }
    public void ContinueGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1;
        bgmSource.Play();//播放场景的BGM音乐
        playerAudio.audioSource2.mute = false;//关闭人物走动声静音
    }
    public void RestartGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1;
        bgmSource.Play();//播放场景的BGM音乐
        playerAudio.audioSource2.mute = false;//关闭人物走动声静音
        SceneController.Instance.RestartGameScene();
    }
    public void QuitGame()
    {
        SceneController.Instance.TransitionToMain();//返回主场景
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
