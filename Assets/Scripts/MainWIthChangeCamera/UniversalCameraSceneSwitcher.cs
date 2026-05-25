using UnityEngine;

/// <summary>
/// 通用相机场景切换器（Animator StateMachineBehaviour）
/// 替代原来的 8 个 ChangeCameraTo*.cs 重复脚本
/// 
/// 使用方式：
/// 1. 在 Animator State 上添加此 Behaviour
/// 2. 在 Inspector 中设置 Target Scene Name（如 "FantasyForest"、"IceAndSnow"）
/// 3. 当该状态退出时自动触发相机场景切换
/// </summary>
public class UniversalCameraSceneSwitcher : StateMachineBehaviour
{
    [Header("目标场景配置")]
    [Tooltip("要切换到的目标场景名称（必须与 CameraChangeManager 中的 key 匹配）")]
    public string targetSceneName = "FantasyForest"; // 默认值示例

    // 缓存引用（避免每帧查找）
    private CameraChangeManager _cameraChangeManager;

    /// <summary>
    /// 状态进入时缓存 CameraChangeManager 引用
    /// </summary>
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _cameraChangeManager = FindFirstObjectByType<CameraChangeManager>();
        
        if (_cameraChangeManager == null)
        {
            Debug.LogError($"[UniversalCameraSceneSwitcher] 未找到 CameraChangeManager！请确保场景中存在该组件。");
        }
    }

    /// <summary>
    /// 状态退出时触发场景切换
    /// </summary>
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 安全性检查
        if (_cameraChangeManager == null)
        {
            Debug.LogWarning($"[UniversalCameraSceneSwitcher] CameraChangeManager 为空，无法切换到场景: {targetSceneName}");
            return;
        }

        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogWarning("[UniversalCameraSceneSwitcher] targetSceneName 未设置！");
            return;
        }

        // 执行场景切换
        _cameraChangeManager.SwitchToScene(targetSceneName);
        
        Debug.Log($"[UniversalCameraSceneSwitcher] 已切换到场景: {targetSceneName}");
    }
}
