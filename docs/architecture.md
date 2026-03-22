# ProtoShift-Core 架构概览

## 分层结构

```
┌────────────────────────────────────────────┐
│             用户（自然语言）                 │
├────────────────────────────────────────────┤
│         OpenCode Host (主壳层)              │
│   会话 · 计划 · 工具调度 · Skills 运行     │
├────────────────────────────────────────────┤
│       ProtoShift Extension (扩展层)         │
│   Agent · Command · Skill · Hook · Plugin  │
├──────────┬─────────────┬───────────────────┤
│ Godot    │ Engine      │ Unreal            │
│ Bridge   │ Bridge      │ Bridge            │
│  MCP/CLI │ (统一语义)  │ RemoteCtrl/CLI    │
├──────────┴──────┬──────┴───────────────────┤
│                 │                           │
│   SharedCore    │     UiSchema              │
│   状态机·任务   │     布局·组件·绑定        │
│   配置·存档     │     主题·事件             │
│   原型描述      │                           │
└─────────────────┴───────────────────────────┘
```

## 项目模块

| 模块 | 路径 | 职责 |
|------|------|------|
| SharedCore | `src/SharedCore/` | 引擎无关的 C# 规则层 |
| EngineBridge | `src/EngineBridge/` | 统一操作语义接口 |
| GodotBridge | `src/GodotBridge/` | Godot MCP/CLI 适配 |
| UnrealBridge | `src/UnrealBridge/` | Unreal Remote Control 适配 |
| UiSchema | `src/UiSchema/` | 通用 UI 描述层 |
| Extension | `.opencode/` | Agent、Command、Skill、Plugin |
| Plugin | `plugins/` | opencode 插件（hooks + tools） |

## 数据流

```
用户输入 → OpenCode Shell
         → ProtoShift Agent 分析意图
         → 选择 Skill（godot-scene / unreal-actor / shared-core / ui-schema / migration）
         → 调用 EngineBridge 统一语义
         → GodotBridge 或 UnrealBridge 执行
         → 结果回传 → Agent 总结 → 用户
```

## 迁移流程

```
Godot 原型 → 导出迁移模型 → 分析共享覆盖率 → Unreal 承接
                                              ├─ SharedCore → UnrealSharp
                                              ├─ 场景树 → Actor/Component
                                              ├─ UiSchema → UMG
                                              └─ GDScript → 需重写部分
```
