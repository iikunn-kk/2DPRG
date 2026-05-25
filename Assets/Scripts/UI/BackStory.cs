using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 背景故事文本显示控制
/// 支持剧情介绍和致谢文本的分段显示
/// </summary>
public class Backstory : MonoBehaviour
{
    [Header("UI组件")]
    public Text BackstroryText;
    public Text ThanksText;

    [Header("文本文件")]
    public TextAsset BackstroryTextFile;
    public TextAsset ThanksTextFile;

    [Header("显示设置")]
    [SerializeField] private float _textSpeed = 0.05f;
    [SerializeField] private float _backStoryDisplayDuration = 25f;
    [SerializeField] private float _thanksDisplayDuration = 13f;
    [SerializeField] private float _fadeOutDelay = 1.01f;

    [Header("面板")]
    public GameObject BackstoryPanel;
    public GameObject ThanksPanel;
    public Animator thanksPanelAnimator;

    private int _index;
    private List<string> _textList = new List<string>();

    public void ShowBackstory()
    {
        if (BackstroryTextFile == null)
        {
            Debug.LogWarning("[Backstory] 背景故事文本文件为空");
            return;
        }
        GetTextFromFile(BackstroryTextFile);
        StartCoroutine(ShowBackstoryText());
        StartCoroutine(DeActiveBackStoryPanel());
    }

    public void ShowThanksText()
    {
        if (ThanksTextFile == null)
        {
            Debug.LogWarning("[Backstory] 致谢文本文件为空");
            return;
        }
        GetTextFromFile(ThanksTextFile);
        StartCoroutine(ShowThanksTextCoroutine());
        StartCoroutine(DeActiveThanksPanel());
    }

    private void GetTextFromFile(TextAsset file)
    {
        _textList.Clear();
        _index = 0;

        var lineContent = file.text.Split('\n');
        foreach (var line in lineContent)
        {
            _textList.Add(line);
        }
    }

    private IEnumerator ShowBackstoryText()
    {
        if (BackstoryPanel != null)
            BackstoryPanel.SetActive(true);

        yield return ShowTextByCharacter(BackstroryText);
    }

    private IEnumerator ShowThanksTextCoroutine()
    {
        if (ThanksPanel != null)
            ThanksPanel.SetActive(true);

        yield return ShowTextByCharacter(ThanksText);
    }

    /// <summary>
    /// 逐字显示文本（使用 StringBuilder 避免 GC 分配）
    /// </summary>
    private IEnumerator ShowTextByCharacter(Text textUI)
    {
        if (textUI == null || _textList.Count == 0) yield break;

        var sb = new StringBuilder();
        string currentLine = _textList[_index];

        for (int i = 0; i < currentLine.Length; i++)
        {
            sb.Append(currentLine[i]);
            textUI.text = sb.ToString();
            yield return new WaitForSeconds(_textSpeed);
        }

        _index++;
    }

    private IEnumerator DeActiveBackStoryPanel()
    {
        yield return new WaitForSeconds(_backStoryDisplayDuration);
        if (BackstoryPanel != null)
            BackstoryPanel.SetActive(false);
    }

    private IEnumerator DeActiveThanksPanel()
    {
        yield return new WaitForSeconds(_thanksDisplayDuration);

        if (thanksPanelAnimator != null)
            thanksPanelAnimator.SetTrigger("isFadeOut");

        yield return new WaitForSeconds(_fadeOutDelay);

        if (ThanksPanel != null)
            ThanksPanel.SetActive(false);
    }
}
