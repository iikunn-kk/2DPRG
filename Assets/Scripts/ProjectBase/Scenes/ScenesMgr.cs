// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Events;
// using UnityEngine.SceneManagement;

// /// <summary>
// /// 场景切换模块
// /// 1、场景异步加载
// /// 2、协程
// /// 3、委托
// /// </summary>
// public class ScenesMgr : BaseManager<ScenesMgr>
// {
//     /// <summary>
//     /// 切换场景 同步加载
//     /// </summary>
//     /// <param name="name"></param>
//     public void LoadScene(string name, UnityAction fun)
//     {
//         //场景同步加载
//         SceneManager.LoadScene(name);
//         //加载完成过后，才会去执行fun
//         fun();
//     }

//     /// <summary>
//     /// 提供给外部调用的异步加载场景的方法
//     /// </summary>
//     /// <param name="name"></param>
//     /// <param name="fun"></param>
//     public void LoadSceneAsync(string name, UnityAction fun)
//     {
//         MonoMgr.Instance.StartCoroutine(ReallyLoadSceneAsyn(name, fun));
//     }

//     /// <summary>
//     /// 协程异步加载场景
//     /// </summary>
//     /// <param name="name"></param>
//     /// <param name="fun"></param>
//     /// <returns></returns>
//     private IEnumerator ReallyLoadSceneAsyn(string name, UnityAction fun)
//     {
//         AsyncOperation ao = SceneManager.LoadSceneAsync(name);

//         //可以得到场景加载的一个进度
//         while (!ao.isDone)
//         {
//             //这里面去更新进度条
//             EventCenter.Instance.EventTrigger("进度条更新", ao.progress);
//             yield return ao.progress;
//         }
//         fun();
//     }
// }
