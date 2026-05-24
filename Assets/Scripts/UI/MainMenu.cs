using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{
    public Button newGameBtn;
    public Button BackStoryBtn;
    public Button continueBtn;
    public Button quitBtn;
    public GameObject backStory;
    // private PlayerInputController inputControl;//输入控制系统

    void Awake()
    {
        // newGameBtn = transform.GetChild(1).GetComponent<Button>();
        // continueBtn = transform.GetChild(2).GetComponent<Button>();
        // quitBtn = transform.GetChild(3).GetComponent<Button>();
        // inputControl = new PlayerInputController();

        // newGameBtn.onClick.AddListener(NewGame);
        // continueBtn.onClick.AddListener(ContinueGame);
        // quitBtn.onClick.AddListener(QuitGame);
        // StartCoroutine("WaitInputSystem");
    }
    // private void OnEnable()
    // {
    //     inputControl.Enable();
    // }
    // private void OnDisable()
    // {
    //     inputControl.Disable();
    // }
    public void NewGame()
    {
        PlayerPrefs.DeleteAll();
        //转换场景
        SceneController.Instance.TransitionToFirstLevel();
    }
    public void BackStory()
    {
        backStory.SetActive(true);
    }
    public void ContinueGame()
    {
        SceneController.Instance.TransitionToLoadGame();
    }
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("退出游戏");
    }
    IEnumerator WaitInputSystem()
    {
        yield return new WaitForSeconds(1f);
        newGameBtn.interactable = true;
        continueBtn.interactable = true;
        quitBtn.interactable = true;
        Debug.Log("等待一秒后刷新三个按键的可互动状态");
    }
}
