using System.Collections;
using System.Collections.Generic;
// using UnityEditor.Animations;
using UnityEngine;

public class CameraChangeManager : MonoBehaviour
{
    // ========== 森林场景引用 ==========
    public GameObject woxForest;
    public GameObject fantasyForest;
    public GameObject forestSpritePack;

    // ========== 冰雪场景引用 ==========
    public GameObject iceAndSnow;
    // ========== 沙漠场景引用 ==========
    public GameObject desert;
    // ========== 火山场景引用 ==========
    public GameObject lavaDungeon;
    public GameObject island;
    public GameObject mounts;

    void Start()
    {
        // 初始状态：启用森林场景，禁用冰雪场景
        woxForest.SetActive(true);
        fantasyForest.SetActive(false);
        forestSpritePack.SetActive(false);
        iceAndSnow.SetActive(false);
        desert.SetActive(false);
        lavaDungeon.SetActive(false);
        island.SetActive(false);
        mounts.SetActive(false);
    }

    // 切换到森林场景的核心方法（由 IceLastAnimBehaviour 调用）
    public void SwitchToFantasyForestScene()
    {
        // 禁用第一个森林场景（相机 + 地图）
        woxForest.SetActive(false);
        // 启动第二个森林场景（相机 + 地图）
        fantasyForest.SetActive(true);
    }
    public void SwitchToForestSpritePack()
    {
        fantasyForest.SetActive(false);
        forestSpritePack.SetActive(true);
    }

    public void SwtichToIceScene()
    {
        //禁用森林场景
        forestSpritePack.SetActive(false);
        //启动冰雪场景
        iceAndSnow.SetActive(true);
    }
    public void SwitchToDesertScene()
    {
        iceAndSnow.SetActive(false);
        desert.SetActive(true);
    }
    public void SwitchToLavaScene()
    {
        iceAndSnow.SetActive(false);
        // desert.SetActive(false);
        lavaDungeon.SetActive(true);
    }
    public void SwitchToIslandScene()
    {
        lavaDungeon.SetActive(false);
        island.SetActive(true);
    }
    public void SwitchToMountsScene()
    {
        island.SetActive(false);
        mounts.SetActive(true);
    }
    public void SwitchBackToWoxForestScene()
    {
        mounts.SetActive(false);
        woxForest.SetActive(true);
    }

}
