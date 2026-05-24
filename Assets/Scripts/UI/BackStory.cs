using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Backstory : MonoBehaviour
{
    [Header("UI组件")]
    public Text BackstroryText;
    public Text ThanksText;

    [Header("文本文件")]
    public TextAsset BackstroryTextFile;
    public TextAsset ThanksTextFile;
    public int index;

    public GameObject BackstoryPanel;
    public GameObject ThanksPanel;

    public Animator thanksPanelAnimator;

    public float textSpeed;
    List<string> textList = new List<string>();


    public void ShowBackstory()
    {
        GetTextFromFile(BackstroryTextFile);
        StartCoroutine(DeActiveBackStoryPanel());
        StartCoroutine(ShowBackstoryText());
    }

    public void ShowThanksText()
    {
        GetTextFromFile(ThanksTextFile);
        StartCoroutine(ShowText());
        StartCoroutine(DeActiveThanksPanel());
    }

    void GetTextFromFile(TextAsset file)
    {
        textList.Clear();
        index = 0;

        var lineContent = file.text.Split("\n");//文本按行分割

        foreach (var line in lineContent)
        {
            textList.Add(line);
        }

    }

    IEnumerator ShowBackstoryText()
    {
        BackstroryText.text = "";
        BackstoryPanel.SetActive(true);
        //gameObject.SetActive(true);
        for (int i = 0; i < textList[index].Length; i++)//每一行所有字符的总长度
        {
            BackstroryText.text += textList[index][i];//把当前行的每个字符一个一个加到（显示）文本上面
            yield return new WaitForSeconds(textSpeed);
        }
        index++;
    }


    IEnumerator ShowText()
    {
        ThanksText.text = "";
        ThanksPanel.SetActive(true);
        //gameObject.SetActive(true);
        for (int i = 0; i < textList[index].Length; i++)//每一行所有字符的总长度
        {
            ThanksText.text += textList[index][i];//把当前行的每个字符一个一个加到（显示）文本上面
            yield return new WaitForSeconds(textSpeed);
        }
        index++;
    }


    IEnumerator DeActiveBackStoryPanel()
    {
        yield return new WaitForSeconds(25f);
        BackstoryPanel.SetActive(false);
    }

    IEnumerator DeActiveThanksPanel()
    {
        yield return new WaitForSeconds(13f);
        thanksPanelAnimator.SetTrigger("isFadeOut");
        //ThanksPanel.SetActive(false);
        Debug.Log("隐藏致谢文本");
        yield return new WaitForSeconds(1.01f);
        ThanksPanel.SetActive(false);
    }




}
