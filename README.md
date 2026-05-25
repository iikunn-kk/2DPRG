<p align="center">
  <h1>⚔️ Oath: Purification Journey</h1>
  <h3>誓约净化之旅</h3>
  <p><em>2D 像素风格横版动作 RPG · Unity 2023</em></p>
</p>

---

## 🎮 游戏简介

**Oath: Purification Journey** 是一款 2D 像素风横版动作 RPG。玩家将穿梭于多个风格迥异的地图，与各类怪物战斗，通过击杀敌人积累经验、升级属性，揭开一段围绕着"誓约"与"净化"的冒险故事。

### 核心特色

| 🗡️ 战斗 | 📈 成长 | 📖 剧情 |
|----------|---------|---------|
| 实时攻击+暴击 | 击杀经验→升级 | NodeCanvas 对话树 |
| 滑铲消耗能量 | 血量/攻击/能量提升 | 18 个剧情文本文件 |
| 二段跳+土狼时间 | 10 级满级 | 多 NPC 交互 |

---

## 🎬 演示截图

> *运行游戏后截取 Main Menu、战斗场景、对话系统等截图，替换以下占位*

```
📸 建议截图：
- Main Menu 主菜单
- 角色在 Training Ground 战斗
- NPC 对话界面
- 场景切换淡入淡出效果
- 多地图展示（火山/雪地/森林）
```

---

## ⌨️ 操作键位

| 按键 | 功能 |
|------|------|
| `W A S D` / 方向键 | 移动 |
| `Space` | 跳跃（支持二段跳） |
| `鼠标左键` | 攻击 |
| `Shift` + 方向键 | 滑铲（消耗能量） |
| `S`（按住） | 蹲下 |
| `Escape` | 暂停菜单 |

---

## 🗺️ 游戏关卡

```
Main Menu（主菜单）
    ↓
Training Ground（训练场）
    ↓
Wox Forest（沃克斯森林）
    ↓
Fantasy Forest（幻境森林）
    ↓
Ice and Snow（冰霜雪原）
    ↓
Desert（沙漠）
    ↓
Island Pack（群岛）
    ↓
Mounts（山脉）
    ↓
Lava Dungeon（熔岩地牢）
    ↓
The Final Episode（最终章）
```

---

## 🛠️ 技术栈

| 技术 | 版本 | 用途 |
|------|------|------|
| **Unity** | 2023.1.1f1 | 游戏引擎 |
| **Cinemachine** | 2.9.7 | 摄像机系统 |
| **Input System** | 1.6.3 | 输入管理 |
| **NodeCanvas** | 商业插件 | 对话树/行为树 |
| **DOTween** | 商业插件 | 补间动画 |
| **2D Tilemap** | 1.0.0 | 关卡搭建 |

---

## 🚀 快速开始

### 环境要求

- **Unity Hub** + **Unity 2023.1.1f1**
- **Windows** 64-bit（当前目标平台）
- 磁盘空间 > 2GB

### 运行步骤

```bash
# 1. 克隆仓库
git clone https://github.com/Coolcoolcoo/Oath-Purification-Journey.git

# 2. 用 Unity Hub 打开项目文件夹

# 3. 等待 Asset 导入完成（首次约 3-5 分钟）

# 4. 打开场景 Assets/Scenes/GameScene/Main.unity

# 5. 点击 Play ▶️ 运行
```

---

## 📁 项目结构

```
Assets/
├── Scripts/                  # C# 源码（93个文件）
│   ├── Manager/              # 核心管理器（Game/Save/Dialogue/Camera）
│   ├── Character/Player/     # 玩家控制/动画/状态机
│   ├── Character/Enemies/    # 11种敌人（通用Fsm驱动）
│   ├── UI/                   # 界面系统（状态条/菜单/对话/渐变）
│   ├── Audio/                # 音效系统（场景自适应跑步音效）
│   ├── Dialogue/             # 对话交互（Npc/PickUp/Interactable）
│   ├── Transition/           # 场景传送
│   ├── SceneManagement/      # 场景加载
│   └── ProjectBase/          # 基础框架（Event/Mono/Res/Music）
│
├── Scenes/GameScene/         # 11个游戏场景
├── Animation/                # 角色/敌人/UI 动画
├── Prefab/                   # 预制体
├── Tilemap/                  # 瓦片地图
├── TheFinalText/             # 18个剧情对话文本
└── Settings/                 # Input System 配置
```

---

## 🏗️ 架构亮点

```
┌─────────────────────────────────────────┐
│  单例管理器层（Singleton/BaseManager）    │
│  GameMgr / SaveMgr / SceneCtrl / UIMgr  │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│  玩家状态机（PlayerController）           │
│  Idle→Run→Jump→Fall→Crouch→Attack→Slide  │
│  →Hurt→Dead→Dialogue（互斥+优先级）       │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│  事件系统（双轨制）                       │
│  EventCenter（代码级）+ SO Event（资产级） │
└─────────────────────────────────────────┘
```

- **存档加密**：XOR 混淆 + PlayerPrefs
- **场景切换**：异步加载 + 淡入淡出 + 2帧等待初始化
- **敌人系统**：通用 EnemyFsm + ScriptableObject 配置驱动
- **对话系统**：NodeCanvas 反射调用 + 状态锁定 + UI 自动恢复

---

## 📝 开发日志

- **2026-05-25** — 93个文件全面代码优化：null安全、死代码清理、事件中心重构、场景切换修复、对话系统完善、蹲下/滑铲逻辑修复
- **之前** — 敌人状态机迁移至通用 EnemyFsm、玩家状态机迁移至 PlayerState 枚举

---

## 📄 许可证

本项目为个人开发项目，未指定开源许可证。如有疑问请联系作者。

---

## 👤 作者

**Coolcoolcoo**

---

<p align="center">
  <sub>Made with ❤️ and Unity</sub>
</p>
