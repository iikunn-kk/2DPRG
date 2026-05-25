using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 相机场景管理器
/// 负责不同场景背景的切换显示（森林/冰雪/沙漠/火山等）
/// </summary>
public class CameraChangeManager : MonoBehaviour
{
    [Header("场景引用")]
    [SerializeField] private GameObject woxForest;
    [SerializeField] private GameObject fantasyForest;
    [SerializeField] private GameObject forestSpritePack;
    [SerializeField] private GameObject iceAndSnow;
    [SerializeField] private GameObject desert;
    [SerializeField] private GameObject lavaDungeon;
    [SerializeField] private GameObject island;
    [SerializeField] private GameObject mounts;

    // 当前激活的场景
    private GameObject _currentScene;

    // 所有场景的字典映射
    private readonly Dictionary<string, GameObject> _sceneMap = new Dictionary<string, GameObject>();

    void Awake()
    {
        // 初始化场景映射
        _sceneMap["WoxForest"] = woxForest;
        _sceneMap["FantasyForest"] = fantasyForest;
        _sceneMap["ForestSpritePack"] = forestSpritePack;
        _sceneMap["IceAndSnow"] = iceAndSnow;
        _sceneMap["Desert"] = desert;
        _sceneMap["LavaDungeon"] = lavaDungeon;
        _sceneMap["Island"] = island;
        _sceneMap["Mounts"] = mounts;
    }

    void Start()
    {
        // 初始状态：只启用 woxForest
        foreach (var scene in _sceneMap.Values)
        {
            if (scene != null)
            {
                scene.SetActive(false);
            }
        }

        SwitchToScene("WoxForest");
    }

    /// <summary>
    /// 通用场景切换方法
    /// </summary>
    /// <param name="sceneName">目标场景名称</param>
    public void SwitchToScene(string sceneName)
    {
        if (!_sceneMap.TryGetValue(sceneName, out var targetScene))
        {
            Debug.LogError($"[CameraChangeManager] 未找到场景: {sceneName}");
            return;
        }

        if (targetScene == null)
        {
            Debug.LogError($"[CameraChangeManager] 场景 {sceneName} 的引用未设置");
            return;
        }

        // 禁用当前场景
        if (_currentScene != null && _currentScene != targetScene)
        {
            _currentScene.SetActive(false);
        }

        // 启用目标场景
        targetScene.SetActive(true);
        _currentScene = targetScene;

        Debug.Log($"[CameraChangeManager] 已切换到场景: {sceneName}");
    }

    // ========== 公开的便捷方法（保持向后兼容）==========

    public void SwitchToFantasyForestScene() => SwitchToScene("FantasyForest");
    public void SwitchToForestSpritePack() => SwitchToScene("ForestSpritePack");
    public void SwitchToIceScene() => SwitchToScene("IceAndSnow");
    public void SwitchToDesertScene() => SwitchToScene("Desert");
    public void SwitchToLavaScene() => SwitchToScene("LavaDungeon");
    public void SwitchToIslandScene() => SwitchToScene("Island");
    public void SwitchToMountsScene() => SwitchToScene("Mounts");
    public void SwitchBackToWoxForestScene() => SwitchToScene("WoxForest");
}
