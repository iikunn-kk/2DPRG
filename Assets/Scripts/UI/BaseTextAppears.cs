using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 文本逐字显示基类
/// 提供通用的文本逐字动画功能，支持从 TextAsset 加载文本
/// 
/// 子类只需重写 OnTextComplete() 方法来定义文本显示完成后的行为
/// </summary>
public abstract class BaseTextAppears : MonoBehaviour
{
    [Header("UI 组件")]
    public Text text; // 显示文本的 UI 组件

    [Header("配置")]
    [SerializeField] protected float showTime = 3f;      // 显示持续时间
    [SerializeField] protected float textSpeed = 0.05f;   // 每个字符显示间隔（秒）

    [Header("数据")]
    public TextAsset textFile; // 文本资源文件
    [HideInInspector] public int index; // 当前行索引（公开供外部访问）

    // 运行时状态
    private List<string> _textList = new List<string>();
    private StringBuilder _stringBuilder = new StringBuilder(); // 避免字符串拼接 GC
    private Coroutine _showTextCoroutine;

    /// <summary>
    /// 对象启用时自动开始显示文本
    /// </summary>
    protected virtual void OnEnable()
    {
        LoadTextFromFile();
        
        if (_showTextCoroutine != null)
            StopCoroutine(_showTextCoroutine);
            
        _showTextCoroutine = StartCoroutine(ShowTextCoroutine());
    }

    /// <summary>
    /// 对象禁用时清理协程
    /// </summary>
    protected virtual void OnDisable()
    {
        if (_showTextCoroutine != null)
        {
            StopCoroutine(_showTextCoroutine);
            _showTextCoroutine = null;
        }
    }

    #region 文本加载

    /// <summary>
    /// 从 TextAsset 加载文本到列表
    /// </summary>
    protected void LoadTextFromFile()
    {
        _textList.Clear();
        index = 0;

        if (textFile == null)
        {
            Debug.LogError($"[{GetType().Name}] TextFile 未设置: {gameObject.name}");
            return;
        }

        // 按行分割文本（兼容 Windows \r\n 和 Linux \n）
        string[] lines = textFile.text.Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.None);
        
        foreach (var line in lines)
        {
            _textList.Add(line.Trim()); // 去除首尾空白
        }
    }

    #endregion

    #region 协程

    /// <summary>
    /// 逐字显示文本协程
    /// </summary>
    private IEnumerator ShowTextCoroutine()
    {
        // 安全检查
        if (text == null)
        {
            Debug.LogError($"[{GetType().Name}] Text 组件未设置: {gameObject.name}");
            yield break;
        }

        if (_textList == null || _textList.Count == 0 || index >= _textList.Count)
        {
            yield break;
        }

        // 清空当前文本
        text.text = "";
        _stringBuilder.Clear();

        string currentLine = _textList[index];
        
        // 逐字显示
        for (int i = 0; i < currentLine.Length; i++)
        {
            _stringBuilder.Append(currentLine[i]);
            text.text = _stringBuilder.ToString(); // 使用 StringBuilder 避免 GC
            
            yield return new WaitForSeconds(textSpeed);
        }

        // 文本显示完成，等待指定时间后隐藏
        yield return new WaitForSeconds(showTime);

        // 调用子类的完成回调
        OnTextComplete();
    }

    #endregion

    #region 子类回调

    /// <summary>
    /// 文本显示完成后的回调
    /// 子类重写此方法来自定义后续行为（如切换场景、显示下一行等）
    /// </summary>
    protected abstract void OnTextComplete();

    /// <summary>
    /// 隐藏当前游戏对象
    /// </summary>
    protected void HideObject()
    {
        gameObject.SetActive(false);
    }

    #endregion
}
