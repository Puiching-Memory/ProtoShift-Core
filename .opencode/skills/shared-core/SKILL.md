---
name: shared-core
description: "SharedCore 共享规则技能：管理跨引擎的 C# 规则层、状态机和数据模型"
---

# SharedCore 共享规则

## 能力

- 创建和修改状态机定义
- 管理任务系统（前置条件、完成回调）
- 定义配置模型和加载策略
- 设计存档结构
- 定义原型描述模型

## 项目位置

`src/SharedCore/` — .NET 8 类库

## 命名空间结构

| 命名空间 | 用途 |
|----------|------|
| `ProtoShift.SharedCore.StateMachine` | 有限状态机 |
| `ProtoShift.SharedCore.Tasks` | 任务系统 |
| `ProtoShift.SharedCore.Config` | 配置加载 |
| `ProtoShift.SharedCore.Save` | 存档管理 |
| `ProtoShift.SharedCore.Prototype` | 原型描述模型 |

## 设计原则

- **零引擎依赖**：不引用 Godot SDK 或 Unreal SDK
- **可测试**：所有逻辑可通过纯 .NET 单元测试验证
- **可序列化**：所有模型支持 JSON 序列化
- **可组合**：状态机、任务、配置可自由组合

## 使用模式

### 在 Godot 中使用

```csharp
using ProtoShift.SharedCore.StateMachine;

public partial class GameManager : Node
{
    private FiniteStateMachine<GameState> _fsm;

    public override void _Ready()
    {
        _fsm = new FiniteStateMachine<GameState>(GameState.Menu);
        _fsm.DefineState(GameState.Menu);
        _fsm.DefineState(GameState.Playing);
        _fsm.AddTransition(GameState.Menu, GameState.Playing);
    }
}
```

### 在 Unreal (UnrealSharp) 中使用

```csharp
using ProtoShift.SharedCore.StateMachine;
using UnrealSharp;

[UClass]
public class AGameManager : AActor
{
    private FiniteStateMachine<GameState> _fsm;

    protected override void BeginPlay()
    {
        _fsm = new FiniteStateMachine<GameState>(GameState.Menu);
        // 与 Godot 中完全一致的 API
    }
}
```
