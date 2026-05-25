using UnityEngine;

/// <summary>
/// 场景退出触发器
/// 当玩家进入触发区域时，自动过渡到目标场景
/// </summary>
public class SceneExit : MonoBehaviour
{
    [Tooltip("目标场景名称")]
    [SerializeField] private string _newSceneName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (string.IsNullOrEmpty(_newSceneName))
        {
            Debug.LogWarning("[SceneExit] 目标场景名称为空");
            return;
        }

        if (SceneLoader.Instance == null)
        {
            Debug.LogError("[SceneExit] SceneLoader 实例不存在");
            return;
        }

        SceneLoader.Instance.TransitionToScene(_newSceneName);
    }
}
