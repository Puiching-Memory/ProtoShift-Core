# ProtoShift-Core

ProtoShift-Core 是一个基于 opencode 二次开发的跨引擎 Agent 工作台。

目标不是把 Godot 脚本翻译成 Unreal 脚本，而是让同一个游戏从原型阶段开始就运行在一份可移植的 C# 游戏包之上。

## 核心路线

- opencode 作为宿主，ProtoShift 通过 `.opencode/` 扩展配置提供产品化工作流
- Godot 作为原型宿主，负责快速生成、运行验证和手感迭代
- Unreal 5 作为承接宿主，通过 UnrealSharp 运行同一份 C# 游戏包
- `standard-template/Core/` 维护模板内共享 core，不再保留仓库根 `src/`
- `standard-template/` 是模板源，普通 agent 任务只应修改 `workspace/` 中复制出的项目
- 每个游戏在 `workspace/<project-slug>/GamePackage/` 中维护自己的可移植玩法代码
- Godot 和 Unreal 只实现一次 Host Runtime，后续游戏复用同一套宿主协议

## CAT 原则

- C - Code Reuse：复用的是项目级 `GamePackage`、ViewModel、Schema 和测试快照，而不是引擎脚本
- A - Adapter Design：适配器是框架级 Host Runtime，不是每个游戏各写一套双端玩法实现
- T - Token-friendly：AI 主要生成 C# 游戏包、Manifest、UiSchema 和绑定配置，而不是依赖像素和手工编辑器步骤

## 架构分层

```text
ProtoShift-Core/
├── .opencode/                         # Agent、命令、skills、MCP 配置
├── standard-template/
│   ├── Core/
│   │   ├── SharedCore/                # 模板内平台级通用能力：状态机、事件、序列化、测试基座
│   │   ├── EngineBridge/              # 编辑器与宿主统一语义
│   │   ├── GodotBridge/               # Godot Host Runtime 与工具桥接
│   │   ├── UnrealBridge/              # Unreal Host Runtime 与工具桥接
│   │   └── UiSchema/                  # 通用 UI 描述与 ViewModel 绑定
│   ├── GamePackage/                   # 模板级默认可移植玩法包
│   ├── GodotHost/                     # Godot 宿主壳层
│   └── UnrealProject/                 # UnrealSharp 承接骨架
├── workspace/
│   └── <project-slug>/
│       ├── GamePackage/               # 该游戏唯一玩法真相，禁止引用 Godot / UnrealSharp
│       ├── GamePackage.Tests/         # 纯 .NET 测试、回放、快照比对
│       ├── GodotHost/                 # Godot 宿主壳层，薄适配
│       ├── Migration/                 # Scene Manifest、Gameplay Spec、UiSchema、backlog
│       └── UnrealProject/             # UnrealSharp 承接工程
├── docs/
├── samples/
└── AGENTS.md
```

## 使用流程

1. 用户用自然语言描述要做的游戏原型。
2. Agent 在 `workspace/<project-slug>/` 初始化 Godot 原型宿主，并先创建 `GamePackage/`。
3. 玩法规则、命令、事件、ViewModel 优先写入 `GamePackage/`，Godot 只负责 Host 接线和运行验证。
4. 稳定后导出 `Scene Manifest`、`Gameplay Spec`、`UiSchema/ViewModel` 与 `host-only backlog`。
5. Agent 使用 unreal-mcp 创建 Unreal 工程骨架，并通过 UnrealSharp 加载同一个 `GamePackage`。
6. Godot 与 Unreal 分别只验证宿主层行为；核心玩法测试始终跑在纯 .NET 层。

## 关键约束

- 不把具体游戏规则写入 `standard-template/Core/SharedCore/`
- 不让项目级玩法代码引用 `Godot.*` 或 `UnrealSharp.*`
- 不把迁移建立在“重写引擎脚本”上，而建立在“同包双宿主运行”上
- 不把 UI 逻辑散落在引擎层；优先输出 ViewModel 与 UiSchema

## 快速开始

```bash
git clone https://github.com/Puiching-Memory/ProtoShift-Core.git
cd ProtoShift-Core
dotnet build ProtoShift.sln
opencode
```

详见 [docs/quickstart.md](docs/quickstart.md)。

## 文档

- [架构概览](docs/architecture.md)
- [统一操作语义](docs/semantics.md)
- [快速开始](docs/quickstart.md)
- [落地蓝图](TODO.md)