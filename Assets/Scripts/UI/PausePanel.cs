using System.Collections;
using UnityEngine;

/// <summary>
/// 暂停面板管理器。
/// 负责暂停/继续游戏时的 BGM 控制和 UI 显示。
/// </summary>
public class PausePanel : Singleton<PausePanel>
{
    public GameObject pausePanel;
    private GameObject _bgm;
    private AudioSource _bgmSource;
    private PlayerAudio _playerAudio;

    private static readonly WaitForSeconds DeadWait = new WaitForSeconds(1f);

    protected override void Awake()
    {
        base.Awake();
        _bgm = GameObject.Find("BGM");

        if (_bgm == null)
            Debug.LogWarning("[PausePanel] 未找到 BGM 对象");
    }

    private void Start()
    {
        if (_bgm != null)
            _bgmSource = _bgm.GetComponent<AudioSource>();

        if (_bgmSource == null)
            Debug.LogWarning("[PausePanel] 未找到 BGM AudioSource");

        // 缓存 PlayerAudio 引用（不再每帧查找）
        _playerAudio = FindFirstObjectByType<PlayerAudio>();
    }

    private void OnEnable()
    {
        // 场景切换后刷新缓存
        _playerAudio = FindFirstObjectByType<PlayerAudio>();
    }

    public void PauseGame()
    {
        if (pausePanel != null)
            pausePanel.SetActive(true);

        Time.timeScale = 0;

        if (_bgmSource != null)
            _bgmSource.Pause();

        MutePlayerAudio(true);
    }

    public void ContinueGame()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        Time.timeScale = 1;

        if (_bgmSource != null)
            _bgmSource.Play();

        MutePlayerAudio(false);
    }

    public void RestartGame()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        Time.timeScale = 1;

        if (_bgmSource != null)
            _bgmSource.Play();

        MutePlayerAudio(false);

        SceneController.Instance?.RestartGameScene();
    }

    public void QuitGame()
    {
        if (SceneController.Instance != null)
            SceneController.Instance.TransitionToMain();
        Time.timeScale = 1;
    }

    public void DeadToRestartGame()
    {
        StartCoroutine(DeadToPauseGame());
    }

    private IEnumerator DeadToPauseGame()
    {
        yield return DeadWait;
        PauseGame();
    }

    private void MutePlayerAudio(bool mute)
    {
        if (_playerAudio != null && _playerAudio.audioSource2 != null)
            _playerAudio.audioSource2.mute = mute;
    }
}
