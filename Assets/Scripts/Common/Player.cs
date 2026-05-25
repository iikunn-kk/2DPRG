using UnityEngine;

/// <summary>
/// 玩家单例组件（跨场景持久化）
/// 注意：这是一个轻量级定位器，主要逻辑在 PlayerController 中
/// </summary>
public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
