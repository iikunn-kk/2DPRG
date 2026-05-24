using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BackStoryTextAppears : MonoBehaviour
{
    [Header("UI组件")]
    public Text text;// 文本物体。
    [Header("参数")]
    public float showTime;//显示时间

    [Header("文本文件")]

    public TextAsset textFile;// 文本资源文件。
    public int index;// 当前显示文本的索引。

    public float textSpeed;// 文本逐个字符显示的速度，单位为秒。

    List<string> textList = new List<string>();// 存储从文本文件中读取的所有文本行的列表。


    // Start is called before the first frame update
    /// <summary>
    /// 在第一帧更新之前调用，进行初始化操作。
    /// </summary>
    void Start()
    {

    }
    private void OnEnable()
    {
        // 从文本文件中读取文本
        GetTextFromFile(textFile);
        // 初始化索引为 0
        index = 0;
        StartCoroutine(ShowText());
        //gameObject.SetActive(false);
    }

    /// <summary>
    /// 从文本资源文件中读取文本并存储到列表中。
    /// </summary>
    /// <param name="file">包含文本的文本资源文件。</param>
    void GetTextFromFile(TextAsset file)
    {
        // 清空文本列表
        textList.Clear();
        // 重置索引为 0
        index = 0;

        // 按行分割文本文件内容
        var lineContent = file.text.Split("\n");//文本按行分割

        // 遍历每一行文本并添加到列表中
        foreach (var line in lineContent)
        {
            textList.Add(line);
        }
    }

    /// <summary>
    /// 协程方法，逐个字符显示木材元素文本。
    /// </summary>
    /// <returns>IEnumerator 对象，用于协程控制。</returns>
    IEnumerator ShowText()
    {
        // 清空背景故事文本组件的内容
        text.text = "";
        // 遍历当前索引对应的文本行的每个字符
        for (int i = 0; i < textList[index].Length; i++)//每一行所有字符的总长度
        {
            // 将当前字符添加到显示文本中
            text.text += textList[index][i];//把当前行的每个字符一个一个加到（显示）文本上面
            // 等待指定的时间后继续执行
            yield return new WaitForSeconds(textSpeed);
            StartCoroutine("SetFalse");
        }
    }
    IEnumerator SetFalse()
    {
        yield return new WaitForSeconds(showTime);
        gameObject.SetActive(false);
    }
}
