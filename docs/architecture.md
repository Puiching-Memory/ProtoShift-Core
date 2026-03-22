# ProtoShift-Core 架构概览

## 设计目标

ProtoShift-Core 的目标不是把 Godot 项目翻译成 Unreal 项目，而是让同一个游戏在原型阶段和承接阶段都运行在同一份可移植 C# 游戏包之上。

因此，仓库必须区分三种层次：

1. 平台级通用核心
2. 项目级可移植游戏包
3. 引擎级宿主 Runtime

## 分层结构

```text
┌────────────────────────────────────────────────────────────┐
│                     用户（自然语言）                        │
├────────────────────────────────────────────────────────────┤
│                   OpenCode Host / ProtoShift              │
│           会话 · 规划 · Skills · MCP/CLI 调度             │
├────────────────────────────────────────────────────────────┤
│                  Framework Host Runtime                   │
│  EngineBridge · GodotBridge · UnrealBridge · UiSchema    │
├───────────────────────────────┬────────────────────────────┤
│      Project Game Package     │      Migration Artifacts   │
│  命令 · 事件 · 状态 · VM      │  Scene Manifest            │
│  规则 · 回放 · 快照测试       │  Gameplay Spec             │
│                               │  UiSchema / host backlog   │
├───────────────────────────────┴────────────────────────────┤
│     Godot Host Runtime              Unreal Host Runtime    │
│  输入 · 物理观测 · 节点同步       输入 · 物理观测 · Actor同步 │
└────────────────────────────────────────────────────────────┘
```

## 模块职责

| 模块              | 路径                                                 | 职责                                                               |
| ----------------- | ---------------------------------------------------- | ------------------------------------------------------------------ |
| SharedCore        | `standard-template/Core/SharedCore/`   | 模板内平台级基础能力：状态机、任务、序列化、快照、基础模型         |
| EngineBridge      | `standard-template/Core/EngineBridge/` | 编辑器与宿主统一语义，不承载具体游戏规则                           |
| GodotBridge       | `standard-template/Core/GodotBridge/`  | Godot MCP/CLI 与 Host Runtime 桥接                                 |
| UnrealBridge      | `standard-template/Core/UnrealBridge/` | unreal-mcp / Remote Control / UnrealSharp 宿主桥接                 |
| UiSchema          | `standard-template/Core/UiSchema/`     | 通用 UI Schema、ViewModel 绑定与主题令牌                           |
| GamePackage       | `workspace/<slug>/GamePackage/`                      | 项目级唯一玩法真相，禁止引用 Godot / UnrealSharp                   |
| GamePackage.Tests | `workspace/<slug>/GamePackage.Tests/`                | 纯 .NET 测试、回放与快照比对                                       |
| GodotHost         | `workspace/<slug>/GodotHost/`                        | Godot 宿主壳层：输入、物理、UI、场景同步                           |
| Migration         | `workspace/<slug>/Migration/`                        | `Scene Manifest`、`Gameplay Spec`、`UiSchema`、`host-only backlog` |
| UnrealProject     | `workspace/<slug>/UnrealProject/`                    | UnrealSharp 承接工程，加载同一份 `GamePackage`                     |

## 关键边界

### 1. `standard-template/Core/SharedCore/` 不是游戏规则仓库

`standard-template/Core/SharedCore/` 只保留平台级能力，例如：

- 状态机
- 任务系统
- 配置与序列化
- 基础快照结构
- 通用协议模型

它不能承载具体游戏规则，否则最终会退化成混杂多个项目逻辑的公共层。

### 2. `GamePackage/` 是每个项目的唯一玩法真相

项目级 `GamePackage/` 包含：

- 玩法规则
- 状态模型
- 命令与事件
- ViewModel
- Snapshot / Replay 测试输入输出

并且遵守以下限制：

- 不引用 `Godot.*`
- 不引用 `UnrealSharp.*`
- 不依赖节点生命周期或 Actor 生命周期
- 不直接读写引擎资源路径和引擎对象树

### 3. Godot / Unreal 是宿主，不是玩法来源

Godot Host 和 Unreal Host 只负责：

- 输入采集
- 时间推进
- 物理反馈采样
- 场景与 UI 同步
- 宿主级调试与日志

它们不负责保存玩法真相。

## 数据流

```text
用户请求
→ ProtoShift Agent 规划
→ 创建 / 修改 GamePackage
→ 导出 Scene Manifest + Gameplay Spec + UiSchema
→ Godot Host 运行并验证原型
→ Unreal Host 通过 UnrealSharp 加载同一份 GamePackage
→ unreal-mcp 生成骨架、输入映射、Actor/UMG 绑定
→ 对比 gameplay snapshot 验证一致性
```

## 迁移流程

```text
Godot 原型阶段
    ├─ 创建 GamePackage
    ├─ 创建 Godot Host 壳层
    ├─ 运行验证与手感迭代
    └─ 保持玩法代码留在 GamePackage

稳定后导出
    ├─ Scene Manifest
    ├─ Gameplay Spec
    ├─ UiSchema / ViewModel
    └─ host-only backlog

Unreal 承接阶段
    ├─ unreal-mcp 创建工程骨架
    ├─ UnrealSharp 加载 GamePackage
    ├─ Manifest → Actor / Component
    ├─ UiSchema → UMG
    └─ host-only backlog 定向重写
```

## 为什么这比双端重写更优

这套结构把引擎差异压缩到框架层的一次性宿主实现中，而不是让每个游戏都维护一套 Godot 玩法脚本和一套 Unreal 玩法脚本。

对 Agent 来说，主要生成目标因此变成：

- 项目级 `GamePackage`
- `Gameplay Spec`
- `Scene Manifest`
- `UiSchema` 与 ViewModel

而不是跨引擎重写行为逻辑。
