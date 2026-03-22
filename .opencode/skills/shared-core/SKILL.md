---
name: shared-core
description: "SharedCore 平台核心技能：管理平台级通用能力与共享协议，不承载具体游戏规则"
---

# SharedCore 平台核心

## 能力

- 创建和修改平台级状态机、任务系统和序列化工具
- 定义通用配置模型、存档结构和协议类型
- 提供 Snapshot / Replay 基础设施
- 定义宿主 Runtime 所需的抽象契约
- 维护平台级原型描述模型与双宿主共享协议基座

## 项目位置

`standard-template/Core/SharedCore/` — .NET 8 类库

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
- **非项目业务层**：不得写入任何具体游戏规则或项目专属领域模型
- **服务双宿主**：这里提供的是所有项目都可复用的基础设施，不是某个项目进入“迁移阶段”后才启用的特殊层

## 严格边界

以下内容不应写入 `standard-template/Core/SharedCore/`：

- 某个具体游戏的回合规则
- 某个项目的球、卡牌、技能、敌人等领域对象
- 某个项目的 UI 文案或 ViewModel 细节
- 某个项目的关卡结构和资源配置

上述内容应写入项目目录下的 `GamePackage/`。

## 使用模式

### 正确用法

```csharp
using ProtoShift.SharedCore.StateMachine;

public sealed class TurnLoop
{
    private FiniteStateMachine<GameState> _fsm;

    public TurnLoop()
    {
        _fsm = new FiniteStateMachine<GameState>(GameState.Menu);
        _fsm.DefineState(GameState.Menu);
        _fsm.DefineState(GameState.Playing);
        _fsm.AddTransition(GameState.Menu, GameState.Playing);
    }
}
```

上例应被项目级 `GamePackage` 复用，而不是直接写在 Godot 节点或 Unreal Actor 中。

### 错误用法

```csharp
// 不要在 SharedCore 中加入任何只为某个项目服务的规则类
// 例如 PoolMatchRules、CardBattleState、ShooterEnemyWave 等。
```
