# Project Overview: Oath Purification Journey (誓约净化之旅)

> **AI 快速上下文索引** — 后续会话可直接引用此文档，避免重复分析整个项目。

---

## 1. 项目简介与核心功能

**项目名称**: Oath: Purification Journey  
**公司**: Coolcoolcoo  
**产品 ID**: `com.Coolcoolcoo.Oath---Purification-Journey`  
**Unity 版本**: 2023.1.1f1  
**目标平台**: Standalone Windows 64-bit (Mono 后端)

### 核心玩法
- 2D 像素风格横版动作 RPG，玩家在多张地图间穿梭冒险
- 战斗系统：实时攻击(暴击/伤害浮动/无敌帧)、滑铲(消耗能量)、二段跳
- 角色成长：击杀敌人获得经验 → 升级提升属性（血量/攻击/暴击/能量）
- 剧情驱动：NodeCanvas 对话树系统推进叙事(`Assets/TheFinalText/` 共 18 个 .txt 文件)
- 场景切换：同场景传送 + 跨场景异步加载 (10 个游戏关卡)

---

## 2. 技术栈与依赖版本

### Unity 内置包

| 包名 | 版本 | 用途 |
|------|------|------|
| `com.unity.inputsystem` | 1.6.3 | 新版输入系统 |
| `com.unity.cinemachine` | 2.9.7 | 摄像机系统 |
| `com.unity.2d.tilemap` | 1.0.0 | 2D Tilemap |
| `com.unity.2d.tilemap.extras` | 4.0.2 | Tilemap 扩展 |
| `com.unity.2d.animation` | 10.0.2 | 2D 骨骼动画 |
| `com.unity.2d.spriteshape` | 10.0.1 | 2D 精灵形状 |
| `com.unity.timeline` | 1.8.8 | Timeline 动画 |
| `com.unity.textmeshpro` | 3.0.9 | 文本渲染 |
| `com.unity.visualscripting` | 1.8.0 | 可视化脚本 (Bolt) |
| `com.unity.collab-proxy` | 2.6.0 | 版本控制 |

### 核心第三方库

| 库 | 形式 | 用途 | 许可证 |
|----|------|------|--------|
| **NodeCanvas** | 完整源码 | 行为树/FSM/对话树可视化框架 | 商业插件 |
| **ParadoxNotion** | 完整源码 | NodeCanvas 底层框架层 | 商业插件(内嵌 Full Serializer: MIT) |
| **DOTween** (Demigiant) | DLL | 补间动画引擎 | 商业/免费 |
| **Proxima Inspector** (v1.5.0) | 完整源码 | 远程运行时调试 | 商业 |
| **BOXOPHOBIC Utils** | 源码 | Inspector 美化工具 | Asset Store |
| **Lucid Editor** | 源码 | Inspector 属性绘制增强 | MIT |
| **Cainos Pixel Art Platformer** | 资源包 | 像素村庄道具美术 | Asset Store |
| **MCPForUnity** | 本地包 | MCP 集成 | 开源 |
| **Colourful Hierarchy** | 编辑器工具 | Hierarchy 彩色标签 | Asset Store |

### 全局预编译宏
- `NODECANVAS` — NodeCanvas 深度集成
- `DOTWEEN` — DOTween 集成

### 程序集结构 (12 个 .csproj)

| 程序集 | 类型 |
|--------|------|
| `Assembly-CSharp` | 主游戏代码 |
| `Assembly-CSharp-Editor` | 编辑器扩展 |
| `Assembly-CSharp-firstpass` | Plugins 优先编译 (含 DOTween 模块) |
| `NodeCanvas` / `ParadoxNotion` | 行为树框架 |
| `Proxima` / `Proxima.Editor` | 远程调试 |
| `Boxophobic.Utils.Scripts/Editor` / `Boxophobic.SkyboxCubemapExtended.Editor` | BOXOPHOBIC 工具 |
| `MCPForUnity.Runtime` / `MCPForUnity.Editor` | MCP 集成 |

---

## 3. 目录结构说明与模块职责

### 3.1 Scripts 目录树

```
Assets/Scripts/
├── Manager/                    ★ 核心管理器
│   ├── GameManager.cs          全局玩家引用(单例)、场景入口查找
│   ├── SaveManager.cs          存档(PlayerPrefs+JsonUtility)、场景名记录
│   ├── DialogueTreeManager.cs  对话期间暂停/恢复移动、播放动画、硬编码定位
│   └── CameraChangeManager.cs  8张地图背景的活性切换
│
├── ProjectBase/                基础框架
│   ├── Base/BaseManager.cs     泛型 C# 单例基类 (非Mono)
│   ├── Event/EventCenter.cs    代码级事件总线 (泛型+Dictionary观察者)
│   └── Mono/MonoController.cs  MonoBehaviour 控制器基类
│
├── Character Stats/            角色数值系统
│   ├── ScriptableObject/
│   │   └── CharacterData_SO.cs 角色静态数据(血量/攻击/暴击/等级/经验)
│   └── MonoBehavior/
│       └── CharacterStats.cs   运行时属性桥接(包装CharacterData_SO的属性读写)
│
├── Character/                  角色逻辑
│   ├── Player/
│   │   ├── PlayerController.cs 玩家输入/移动/跳跃/滑铲/土狼时间(18KB核心)
│   │   └── PlayerAnimation.cs  玩家动画控制器
│   └── Enemies/                8种敌人状态机 (每种含 Patrol/Chase/Attack/Hurt/Dead)
│       ├── FrostSmallDragon/
│       ├── IceSnowDemon/
│       ├── IceSnowMushrooms/
│       ├── IceWolf/
│       ├── LavaDemon/
│       ├── LavaScorpion/
│       ├── LavaDragon/
│       └── MagmaDemon/
│
├── Common/                     公共组件
│   ├── Character.cs            实体基类(生命/受伤/死亡/能量/陷阱逻辑)
│   ├── Attack.cs               攻击伤害源组件
│   └── PhysicsCheck.cs         物理检测(接地/碰墙检测)
│
├── Transition/                 场景过渡
│   └── SceneController.cs      场景控制器(同/跨场景传送、主菜单、重新开始)
│
├── Dialogue/                   对话系统
│   └── DialogueTree/           NodeCanvas 对话树运行时
│
├── UI/                         界面系统
│   └── UIManager.cs            UI管理器(血条更新、存档快捷键、属性面板)
│
├── Audio/                      音频
│   ├── AudioManager.cs         音频管理器(当前为空壳)
│   └── AttackAudio*.cs         攻击音效
│
├── MainWIthChangeCamera/       场景地图切换逻辑
├── ScriptableObject/           SO事件资产
│   ├── VoidEventSO.cs          无参事件SO (CreateAssetMenu: "Event/VoidEventSO")
│   └── CharacterEventSO.cs     带Character参数事件SO (CreateAssetMenu: "Event/CharacterEventSO")
│
├── Tools/
│   └── Singleton.cs            MonoBehaviour 泛型单例基类
│
└── Utilities/                  工具脚本
```

### 3.2 Assets 资源目录

| 目录 | 内容 |
|------|------|
| `Assets/Animation/` | Player, Enemies, Environment, NPC, UI, VFX 动画 |
| `Assets/Scenes/GameScene/` | 11 个游戏场景 (Main, Wox Forest, Fantasy forest, Desert, Ice and snow, Island pack, Lava dungeon, Mounts, Training Ground, The Final Episode, SampleScene) |
| `Assets/Game Data/Character Data/` | 22 个 ScriptableObject 角色数据资产(1 玩家 + 21 怪物) |
| `Assets/Prefab/` | 预制体 (Player, Enemies, Item, NPC, Dialogue, Environment) |
| `Assets/TheFinalText/` | 18 个 .txt 剧情对话文本文件 |
| `Assets/Tilemap/` | 瓦片地图素材 |
| `Assets/TimeLine/` | Timeline 资源 (TheFinal.playable) |
| `Assets/VFX/` | 视觉特效预制体 |
| `Assets/Events/` | ScriptableObject 事件实例 .asset 文件 |
| `Assets/Settings/Input System/` | InputSystem 配置 |
| `Assets/Plugins/Demigiant/DOTween/` | DOTween DLL |
| `Assets/ParadoxNotion/` | NodeCanvas 框架 |
| `Assets/Proxima/` | Proxima 远程调试 |
| `Assets/Cainos/` | Village Props 美术 + Lucid Editor |
| `Assets/开发资源/` | 原始美术/音效素材包 |
| `Assets/对话/` | 对话树文件 (.DT) |

---

## 4. 核心业务逻辑与关键数据流

### 4.1 单例架构

项目有两套单例模式:

**MonoBehaviour 单例** (`Tools/Singleton.cs`):

```csharp
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
// Instance 属性、Awake 去重、OnDestroy 清理
```

使用类: `GameManager`, `SaveManager`, `DialogueTreeManager`, `SceneController`, `UImanager`, `PlayerController`

**C# 单例** (`ProjectBase/Base/BaseManager.cs`):

```csharp
public class BaseManager<T> where T : class, new()
// Instance 属性 + GetInstance() 方法
```

使用类: `EventCenter` (事件中心)

### 4.2 事件系统 (双轨制)

```
┌───────────────────────────────────────┐
│  EventCenter (代码级)                   │
│  EventCenter.cs                        │
│  - AddEventListener<T>(name, action)   │
│  - RemoveEventListener<T>(name, action)│
│  - EventTrigger<T>(name, info)         │
│  - Clear()                             │
│  泛型 + Dictionary<string, IEventInfo> │
│  典型使用: PhysicsCheck↔Player 通信     │
└───────────────────────────────────────┘

┌───────────────────────────────────────┐
│  SO Event (资产级)                      │
│  VoidEventSO: 无参事件                  │
│  CharacterEventSO: Character 参数事件   │
│  通过 Inspector 拖拽绑定                │
│  典型使用: 生命值变化 → UI 更新         │
└───────────────────────────────────────┘

┌───────────────────────────────────────┐
│  Component UnityEvent (组件级)          │
│  Character 类内置:                      │
│  - OnHealthChange (UnityEvent<Character>) │
│  - OnTakeDamage (UnityEvent<Transform>)   │
│  - OnDie (UnityEvent)                  │
│  通过 Inspector 或代码绑定              │
└───────────────────────────────────────┘
```

### 4.3 数据持久化流程

```
保存流程:
  UI快捷键(键盘)/场景切换 → SaveManager.SavePlayerData()
    → JsonUtility.ToJson(GameManager.characterStats.characterData)
    → PlayerPrefs.SetString(key, jsonData)
    → PlayerPrefs.SetString(sceneNameKey, SceneManager.GetActiveScene().name)
    → PlayerPrefs.Save()

加载流程:
  场景进入/PlayerController.Start() → SaveManager.LoadPlayerData()
    → PlayerPrefs.GetString(key)
    → JsonUtility.FromJsonOverwrite(json, characterData)
    → 背包数据通过 CharacterData_SO 同步恢复
```

**保存数据载体**: `CharacterData_SO` (ScriptableObject → JSON)  
**额外记录**: 当前场景名 (用于"继续游戏"功能)

**触发点**:
| 触发位置 | 操作 |
|----------|------|
| `SceneController.Transition()` | 自动保存 → 加载新场景 |
| `SceneController.LoadMain()` | 保存 → 返回主菜单 |
| `UIManager` 键盘事件 | 手动保存/加载 |
| `PlayerController.Start()` | 场景进入时加载数据 |

### 4.4 场景切换流程

```
TransitionToDestination(TransitionPoint)
  ├── SameScene: 同场景传送
  │     └── 更新玩家 position → 直接传送
  └── DifferentScene: 跨场景
        ├── SaveManager.SavePlayerData()
        ├── SceneFader.FadeOut(2s)
        ├── SceneManager.LoadSceneAsync(targetScene)
        ├── Instantiate(playerPrefab, destination)
        ├── SaveManager.LoadPlayerData()
        └── SceneFader.FadeIn(2s)
```

特殊场景切换路径:
- `TransitionToFirstLevel()` → 加载 "Training Ground"
- `TransitionToLoadGame()` → 加载 `SaveManager.SceneName` 记录的场景
- `TransitionToMain()` → 加载 "Main" (保存数据)
- `RestartGameScene()` → 重新加载当前场景

### 4.5 战斗伤害计算流程

```
攻击者 (Attack) 碰撞受击者 (Character)
  └─→ Character.TakeDamage(attacker)
        ├── invulnerable? → 跳过
        ├── CriticalDamage(attacker)
        │     ├── 基础伤害 = Random(minDamage, maxDamage)
        │     └── 暴击判断 → damage × CriticalMultiplier
        ├── 当前生命 > 最终伤害 → 扣血 + TriggerInvulnerable()
        ├── 当前生命 ≤ 最终伤害 → 死亡
        │     └── characterStats.CurrentHealth = 0
        │     └── attacker 获得经验: attacker.characterStats.characterData.UpdateExp(killPoint)
        └── OnHealthChange?.Invoke(this) → UI 更新血条
```

### 4.6 角色升级链

```
击杀敌人 → UpdateExp(killPoint)
  → currentExp += point
  → currentExp >= baseExp → LevelUp()
    → currentLevel = Clamp(currentLevel+1, 0, maxLevel)
    → baseExp += baseExp × LevelMultiplier
    → maxHealth ×= LevelMultiplier, currentHealth = maxHealth
    → minDamage += 1, maxDamage += 1
    → maxPower += 2.5f, currentPower = maxPower
```

---

## 5. API 接口与关键函数签名

### 5.1 GameManager (全局游戏管理)

```csharp
public class GameManager : Singleton<GameManager>
{
    public static CharacterStats PlayerStats { get; }         // 静态快捷访问
    public CharacterStats characterStats;                       // 当前玩家角色
    public void RigisterPlayer(CharacterStats player);         // 注册玩家
    public Transform GetEntrance();                            // 获取场景 ENTER 传送点
}
```

### 5.2 SaveManager (存档管理)

```csharp
public class SaveManager : Singleton<SaveManager>
{
    public string SceneName { get; }                           // 读取上次存档的场景名
    public void SavePlayerData();                              // 保存玩家数据
    public void LoadPlayerData();                              // 加载玩家数据
    public void Save(Object data, string key);                 // 通用保存 (JSON→PlayerPrefs)
    public void Load(Object data, string key);                 // 通用加载 (PlayerPrefs→JSON覆盖)
}
```

### 5.3 SceneController (场景控制)

```csharp
public class SceneController : Singleton<SceneController>
{
    public GameObject playerPrefab;                            // 玩家预制体
    public SceneFader sceneFaderPrefab;                        // 淡入淡出预制体
    public void TransitionToDestination(TransitionPoint tp);   // 传送入口
    public IEnumerator Transition(string sceneName, DestinationTag tag); // 场景过渡协程
    public void TransitionToFirstLevel();                      // 新游戏 → 训练场
    public void TransitionToLoadGame();                        // 继续游戏
    public void TransitionToMain();                            // 返回主菜单
    public void RestartGameScene();                            // 重新开始
}
```

### 5.4 Character (角色实体基类)

```csharp
public class Character : MonoBehaviour
{
    // 属性
    public float maxHealth, currentHealth;                     // 生命值
    public float maxPower, currentPower;                       // 能量值
    public float powerRecoverSpeed;                            // 能量恢复速度
    public float invulnerableDuration;                         // 无敌持续时间
    public bool invulnerable;                                  // 是否无敌
    public CharacterStats characterStats;                      // 关联的数值组件

    // UnityEvent
    public UnityEvent<Character> OnHealthChange;               // 生命值变化
    public UnityEvent<Transform> OnTakeDamage;                 // 受击
    public UnityEvent OnDie;                                   // 死亡

    // 关键方法
    public void TakeDamage(Attack attacker);                   // 受击逻辑 (伤害/暴击/无敌/死亡)
    public float CriticalDamage(Attack attacker);              // 暴击伤害计算
    public void TriggerInvulnerable();                         // 激活无敌帧
    public void LevelUpProperty();                             // 升级属性同步
    public void LogicOfPlayerFallingIntoBloodLossTrap();       // 扣血陷阱
    public void LogicOfPlayerFallingIntoDeathTrap();           // 致死陷阱
    public void OnSlide(float cost);                           // 滑铲消耗能量
}
```

### 5.5 CharacterStats (运行时数值桥接)

```csharp
public class CharacterStats : MonoBehaviour
{
    public CharacterData_SO templateData;         // 模板数据
    public CharacterData_SO characterData;        // 运行时数据(实例化副本)

    // 属性包装器(均支持读写)
    public int MaxHealth { get; set; }
    public int CurrentHealth { get; set; }
    public float MinDamage { get; set; }
    public float MaxDamage { get; set; }
    public float CriticalMultiplier { get; set; }
    public float CirticalChance { get; set; }
    public int CurrentLevel { get; set; }
    public int BaseExp { get; set; }
    public int CurrentExp { get; set; }
    public float CurrentPower { get; set; }
    public float MaxPower { get; set; }
}
```

### 5.6 CharacterData_SO (静态数据模板)

```csharp
[CreateAssetMenu(menuName = "Character Stats/Data")]
public class CharacterData_SO : ScriptableObject
{
    int maxHealth, currentHealth;
    float minDamage, maxDamage, criticalMultiplier, cirticalChance;
    float maxPower, currentPower;
    int killPoint;                                     // 击杀奖励经验
    int currentLevel, maxLevel, baseExp, currentExp;
    float levelBuff;                                   // 每级属性提升倍数
    float LevelMultiplier { get; }                     // 等级系数 = 1 + (level-1)*levelBuff
    void UpdateExp(int point);                         // 增加经验(自动判断升级)
}
```

### 5.7 EventCenter (事件中心)

```csharp
public class EventCenter : BaseManager<EventCenter>
{
    // 泛型事件
    void AddEventListener<T>(string name, UnityAction<T> action);
    void RemoveEventListener<T>(string name, UnityAction<T> action);
    void EventTrigger<T>(string name, T info);

    // 无参事件
    void AddEventListener(string name, UnityAction action);
    void RemoveEventListener(string name, UnityAction action);
    void EventTrigger(string name);

    void Clear();   // 清空所有监听(场景切换时调用)
}
```

### 5.8 UImanager (UI 管理器)

```csharp
public class UImanager : Singleton<UImanager>
{
    public PlayerStatBar playerStatBar;                // 玩家状态条引用
    public CharacterEventSO healthEvent;               // 生命值变化 SO 事件
    public CharacterEventSO updateHealthBar;           // (保留)更新血条 SO 事件

    // 输入绑定:
    //   Save 按键 → SavePlayerData()
    //   Load 按键 → LoadPlayerData()
    //   PausePanel 按键 → PauseGame()
    //   PropertiesPanel 按键 → 切换属性面板显示
}
```

### 5.9 DialogueTreeManager (对话系统管理)

```csharp
public class DialogueTreeManager : Singleton<DialogueTreeManager>
{
    void OndialogueStopMove();          // 对话中暂停移动
    void OndialogueRecoverMove();       // 对话结束恢复移动
    void PlayDialogueWithAttack1/2();   // 播放攻击动画
    void PlayDialogueWithJump/Hurt/Land();
    void MakePlayerJump();              // 给玩家施加跳跃力
    void SetPlayerPositionUp/ToTop/ToVolcano(); // 硬编码坐标定位
}
```

### 5.10 Singleton 基类

```csharp
// MonoBehaviour 单例
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T Instance { get; }
    public static bool IsInitialized { get; }
    // Awake: 如果已存在则 Destroy(gameObject); OnDestroy: 清理引用
}

// C# 单例
public class BaseManager<T> where T : class, new()
{
    public static T Instance { get; }
    public static T GetInstance();
}
```

---

## 6. 环境配置与运行部署步骤

### 6.1 开发环境要求

| 组件 | 版本/要求 |
|------|----------|
| Unity Editor | 2023.1.1f1 |
| 脚本后端 | Mono |
| 目标平台 | Windows Standalone 64-bit |
| 输入系统 | Input System (New) 1.6.3 |
| .NET 兼容性 | .NET Framework (Mono) |

### 6.2 项目启动

1. 用 Unity Hub 打开项目根目录，确保 Unity 版本匹配 (2023.1.1f1)
2. 首次打开需等待资源导入完成
3. 项目主场景: `Assets/Scenes/GameScene/Main.unity`
4. 按 Play 即可运行

### 6.3 输入系统配置

项目使用 Unity New Input System (`com.unity.inputsystem: 1.6.3`)。
配置文件位于 `Assets/Settings/Input System/`。
自动生成的 C# 类: `PlayerInputController`。

默认键位:
- 移动: WASD / 方向键
- 跳跃: Space
- 攻击: 鼠标左键
- 滑铲: Shift + 方向键
- 存档: (由 UIManager 绑定，代码中使用 I/O 键)
- 暂停: Escape

### 6.4 场景结构

```
Main.unity  ← 主菜单入口
  ├── 新游戏 → Training Ground → ... → The Final Episode
  ├── 继续游戏 → 恢复上次存档场景
  └── 退出

游戏关卡 (10个):
  Training Ground > Wox Forest > Fantasy forest > Ice and snow
  > Desert > Island pack > Mounts > Lava dungeon > The Final Episode
```

### 6.5 存档数据存储位置

- 技术方案: Unity `PlayerPrefs` (注册表/文件系统)
- 存档 Key: 角色数据的 ScriptableObject 名称
- 场景 Key: `SaveManager.sceneName` 变量值
- 格式: JSON (通过 `JsonUtility`)

### 6.6 注意事项

1. **MCPForUnity 本地路径**: `manifest.json` 中 `com.coplaydev.unity-mcp` 使用绝对本地路径 `D:/下载/...`，跨机器需要修改或移除
2. **开发资源残留**: `Assets/开发资源/` 下存在 `.downloading` 占位文件 (~269 MB)，与开发无关可清理
3. **过时 API**: `SceneController` 使用 `FindObjectsOfType<T>()` (Obsolete)，需迁移到 `FindObjectsByType<T>(FindObjectsSortMode.None)` (Unity 2023.1+)
4. **硬编码值**: `DialogueTreeManager` 和 `CameraChangeManager` 包含硬编码坐标/场景名，维护需注意
5. **全局宏**: `NODECANVAS` 和 `DOTWEEN` 为全局定义，所有 .csproj 均包含
6. **AudioManager 空壳**: 音频管理器当前未实现具体逻辑
