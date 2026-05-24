// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// /// <summary>
// /// 1、Input类
// /// 2、事件中心模块
// /// 3、公共Mono模块的使用
// /// </summary>
// public class InputMgr :BaseManager<InputMgr>
// {
//     private bool isStart = false;
//     /// <summary>
//     /// 构造函数中 添加Update监听
//     /// </summary>
//     public InputMgr()
//         {
//         MonoMgr.GetInstance().AddUpdateListener(MyUpdate);
//         }

//     public void StartOrEndCheck(bool isOpen)
//     {
//         isStart = isOpen;
//     }

//         private void CheckKeyCode(KeyCode key)
//         {
//         if (!isStart)
//             return;
//         if (Input.GetKeyDown(key))
//         {
//             //事件中心模块 分发按下抬起事件
//             EventCenter.GetInstance().EventTrigger("某键按下", key);
//         }
//         if (Input.GetKeyUp(key))
//         {
//             //事件中心模块 分发按下抬起事件
//             EventCenter.Instance.EventTrigger("某键抬起", key);
//         }
//         }

//         private void MyUpdate()
//         {
//         CheckKeyCode(KeyCode.W);
//         CheckKeyCode(KeyCode.A);
//         CheckKeyCode(KeyCode.S);
//         CheckKeyCode(KeyCode.D);
//     }
// }
