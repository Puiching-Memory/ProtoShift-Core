# ProtoShift-Core

ProtoShift-Core 是一个基于 opencode 二次开发的跨引擎 Agent 工作台。

目标很直接：

- 用户始终通过自然语言使用同一个 opencode 风格入口
- Agent 先在 Godot 中完成游戏原型开发
- 同一项目中的共享逻辑再迁移到 Unreal 5 继续开发
- 全程优先复用 C# 代码、工具协议和 UI 描述

## 核心路线

- opencode 作为独立 CLI 工具安装，ProtoShift 通过 `.opencode/` 扩展配置做产品化封装
- Godot 作为原型引擎，优先承担快速生成、迭代、测试和验证
- Unreal 5 作为承接引擎，通过 UnrealSharp 接入共享 C# 核心
- Shared Core 只放引擎无关的规则、状态、数据模型和 UI Schema
- Godot/Unreal 通过各自 MCP、CLI 或编辑器桥接层接入统一语义

## ACT 原则

- A - Adapter First：先定义统一语义，再接 Godot 和 Unreal 的工具能力
- C - Code Reuse：规则层、状态层、数据层、UI 描述层尽量共享
- T - Tool Driven：全程通过 opencode + Skills + MCP/CLI 驱动，不把工作流建立在手工点编辑器上

## 项目结构

```
ProtoShift-Core/
├── .opencode/                   # opencode 扩展配置
│   ├── opencode.jsonc           # 主配置（MCP、Skills 路径）
│   ├── agent/                   # Agent 定义
│   │   ├── protoshift.md        # 主 Agent
│   │   ├── godot.md             # Godot 专家子 Agent
│   │   └── unreal.md            # Unreal 专家子 Agent
│   ├── command/                 # 命令定义
│   │   ├── prototype.md         # /prototype — 创建原型
│   │   ├── migrate.md           # /migrate — 迁移到 Unreal
│   │   ├── test.md              # /test — 分层测试
│   │   └── engine.md            # /engine — 统一引擎操作
│   └── skills/                  # 技能定义
│       ├── godot-scene/         # Godot 场景管理
│       ├── unreal-actor/        # Unreal Actor 管理
│       ├── shared-core/         # SharedCore 规则管理
│       ├── ui-schema/           # UI Schema 管理
│       └── migration/           # 迁移管理
├── plugins/
│   └── protoshift.ts            # opencode 插件（hooks + tools）
├── src/
│   ├── SharedCore/              # 跨引擎共享 C# 规则层
│   │   ├── StateMachine/        # 有限状态机
│   │   ├── Tasks/               # 任务系统
│   │   ├── Config/              # 配置加载
│   │   ├── Save/                # 存档管理
│   │   └── Prototype/           # 原型描述模型
│   ├── EngineBridge/            # 统一操作语义接口
│   ├── GodotBridge/             # Godot MCP/CLI 适配
│   ├── UnrealBridge/            # Unreal Remote Control 适配
│   └── UiSchema/                # 通用 UI 描述层
├── docs/                        # 文档
│   ├── architecture.md          # 架构概览
│   ├── semantics.md             # 统一操作语义参考
│   └── quickstart.md            # 快速开始
├── samples/                     # 样例
│   ├── migration-model/         # 迁移模型样例
│   ├── shared-core-usage/       # SharedCore 使用样例
│   └── ui-schema/               # UI Schema 样例
├── AGENTS.md                    # ProtoShift 系统指令
├── ProtoShift.sln               # .NET 解决方案
├── Directory.Build.props        # 全局构建属性
└── TODO.md                      # 执行方案说明
```

## 快速开始

```bash
# 克隆
git clone https://github.com/Puiching-Memory/ProtoShift-Core.git
cd ProtoShift-Core

# 安装 opencode（需要 bun）
curl -fsSL https://opencode.ai/install | bash

# 构建 C# 解决方案
dotnet build ProtoShift.sln
```

详见 [docs/quickstart.md](docs/quickstart.md)。

## 产品使用流程

1. 用户进入定制化的 opencode Shell。
2. 用户用自然语言描述要做的游戏原型。
3. Agent 调用 Godot MCP/CLI 创建场景、脚本、资源绑定和玩法逻辑。
4. 共享玩法规则写入 Shared Core，避免锁死在 Godot 引擎层。
5. 当原型稳定后，Agent 在同一项目中调用 Unreal MCP/CLI 和 UnrealSharp，把共享逻辑迁移到 Unreal 5。
6. 用户继续通过同一个自然语言入口推进后续开发，而不是切换一套全新的工具链。

## 最小架构

- **opencode**：主宿主层，负责会话、计划、工具调度和 Skills 运行
- **OpenCode Extension**：ProtoShift 自定义 Agent、命令、hooks 和工作流约束
- **Engine Bridge**：统一 Godot / Unreal 的操作语义，对下接 MCP、CLI、Python 或插件桥接
- **Shared Core**：共享 C# 规则层、状态层、数据模型和迁移模型
- **UI Schema**：通用 UI 描述层，在 Godot 和 Unreal 各自映射为原生 UI 系统

## 文档

- [架构概览](docs/architecture.md)
- [统一操作语义](docs/semantics.md)
- [快速开始](docs/quickstart.md)
- [执行方案](TODO.md)