# ProtoShift-Core

ProtoShift-Core 是一个基于 opencode 二次开发的跨引擎 Agent 工作台。

目标很直接：

- 用户始终通过自然语言使用同一个 opencode 风格入口
- Agent 先在 Godot 中完成游戏原型开发
- 同一项目中的共享逻辑再迁移到 Unreal 5 继续开发
- 全程优先复用 C# 代码、工具协议和 UI 描述

## 核心路线

- 以 git submodule 方式集成 opencode，直接在其 Agent Shell、Skills、MCP 和 CLI 扩展能力上做产品化封装
- Godot 作为原型引擎，优先承担快速生成、迭代、测试和验证
- Unreal 5 作为承接引擎，通过 UnrealSharp 接入共享 C# 核心
- Shared Core 只放引擎无关的规则、状态、数据模型和 UI Schema
- Godot/Unreal 通过各自 MCP、CLI 或编辑器桥接层接入统一语义

## ACT 原则

- A - Adapter First：先定义统一语义，再接 Godot 和 Unreal 的工具能力
- C - Code Reuse：规则层、状态层、数据层、UI 描述层尽量共享
- T - Tool Driven：全程通过 opencode + Skills + MCP/CLI 驱动，不把工作流建立在手工点编辑器上

## 产品使用流程

1. 用户进入定制化的 opencode Shell。
2. 用户用自然语言描述要做的游戏原型。
3. Agent 调用 Godot MCP/CLI 创建场景、脚本、资源绑定和玩法逻辑。
4. 共享玩法规则写入 Shared Core，避免锁死在 Godot 引擎层。
5. 当原型稳定后，Agent 在同一项目中调用 Unreal MCP/CLI 和 UnrealSharp，把共享逻辑迁移到 Unreal 5。
6. 用户继续通过同一个自然语言入口推进后续开发，而不是切换一套全新的工具链。

## 最小架构

- opencode：主宿主层，负责会话、计划、工具调度和 Skills 运行
- OpenCode Extension：ProtoShift 自定义指令、命令、hooks 和工作流约束
- Engine Bridge：统一 Godot / Unreal 的操作语义，对下接 MCP、CLI、Python 或插件桥接
- Shared Core：共享 C# 规则层、状态层、数据模型和迁移模型
- UI Schema：通用 UI 描述层，在 Godot 和 Unreal 各自映射为原生 UI 系统

## 当前仓库方向

- `opencode/` 作为 git submodule 集成上游宿主
- 根仓库负责 ProtoShift 自己的扩展层、桥接层、共享层和文档
- 后续优先补齐目录结构、桥接协议和最小可运行工作流