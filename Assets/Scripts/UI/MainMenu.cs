using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 主菜单管理器
/// 处理新游戏、继续游戏、背景故事、退出等功能
/// </summary>
public class MainMenu : MonoBehaviour
{
    [Header("按钮引用")]
    public Button newGameBtn;
    public Button backStoryBtn;
    public Button continueBtn;
    public Button quitBtn;

    [Header("UI 面板")]
    public GameObject backStory; // 背景故事面板

    void Start()
    {
        InitializeButtons();
    }

    /// <summary>
    /// 初始化按钮状态
    /// </summary>
    private void InitializeButtons()
    {
        // 可选：延迟启用按钮（等待输入系统就绪）
        StartCoroutine(EnableButtonsDelayed());
    }

    /// <summary>
    /// 延迟启用按钮交互（防止输入系统未就绪时的误操作）
    /// </summary>
    private IEnumerator EnableButtonsDelayed()
    {
        yield return new WaitForSeconds(0.5f);

        SetButtonsInteractable(true);
        
        Debug.Log("[MainMenu] 按钮已启用");
    }

    /// <summary>
    /// 设置所有按钮的交互状态
    /// </summary>
    /// <param name="interactable">是否可交互</param>
    private void SetButtonsInteractable(bool interactable)
    {
        if (newGameBtn != null) newGameBtn.interactable = interactable;
        if (continueBtn != null) continueBtn.interactable = interactable;
        if (quitBtn != null) quitBtn.interactable = interactable;
    }

    #region 公共方法（绑定到 UI 按钮）

    /// <summary>
    /// 开始新游戏
    /// 删除存档并跳转到第一关
    /// </summary>
    public void NewGame()
    {
        PlayerPrefs.DeleteAll();
        
        if (SceneController.Instance != null)
        {
            SceneController.Instance.TransitionToFirstLevel();
        }
        else
        {
            Debug.LogError("[MainMenu] SceneController.Instance 为空");
        }
    }

    /// <summary>
    /// 显示背景故事面板
    /// </summary>
    public void BackStory()
    {
        if (backStory != null)
        {
            backStory.SetActive(true);
        }
        else
        {
            Debug.LogWarning("[MainMenu] 背景故事面板引用未设置");
        }
    }

    /// <summary>
    /// 继续上次的游戏进度
    /// </summary>
    public void ContinueGame()
    {
        if (SceneController.Instance != null)
        {
            SceneController.Instance.TransitionToLoadGame();
        }
        else
        {
            Debug.LogError("[MainMenu] SceneController.Instance 为空");
        }
    }

    /// <summary>
    /// 退出游戏
    /// 注意：在编辑器模式下无法真正退出，仅停止播放
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("[MainMenu] 退出游戏");
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    #endregion
}
