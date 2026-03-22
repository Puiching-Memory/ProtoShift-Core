---
name: game-package
description: "项目级可移植游戏包技能：创建和维护 GamePackage、ViewModel、快照测试与迁移产物"
---

# GamePackage 管理

## 能力

- 创建项目级 `GamePackage/` 与 `GamePackage.Tests/`
- 设计命令、事件、状态、领域模型和 ViewModel
- 保持玩法逻辑对 Godot / UnrealSharp 零直接依赖
- 为 Godot Host 和 Unreal Host 提供统一输入输出契约，并从原型第一天起保证同一个 `GamePackage` 被双宿主承接
- 维护当前新游戏的 Snapshot / Replay 测试
- 导出并持续更新 `Gameplay Spec`、UiSchema、README 与 `host-only backlog`
- 检查 Godot Host、Unreal 承接骨架和项目文档是否仍然围绕同一个玩法契约

## 项目位置

- `workspace/<project-slug>/GamePackage/`
- `workspace/<project-slug>/GamePackage.Tests/`
- `workspace/<project-slug>/Migration/`

## 设计原则

- **项目级唯一真相**：具体玩法规则只写在 `GamePackage/`
- **双宿主优先**：从原型开始就按 Godot 和 Unreal 共同承接同一份 `GamePackage` 设计，不把 Unreal 适配延后到独立阶段
- **零宿主依赖**：禁止引用 `Godot.*`、`UnrealSharp.*`
- **命令驱动**：输入进入命令，输出变成状态变化、事件、ViewModel 与表现请求
- **游戏专属测试**：`GamePackage.Tests/` 必须验证当前游戏的真实玩法，不保留模板运行时和模板快照作为主验证手段
- **文档同步**：README、Gameplay Spec、Scene Manifest、UiSchema 和输入映射必须随玩法契约一起更新
- **快照优先**：重要流程必须可通过回放与快照断言
- **单一工作流**：不要把 GamePackage 工作拆成先做 Godot 原型、后做 Unreal 适配的两段式流程

## 初始化检查

开始写 `GamePackage/` 之前，先确认：

- `GamePackage.Tests/` 已存在
- `Migration/` 已存在
- Godot 场景主脚本连接到 `GodotHost/` 而不是旧 `scripts/`
- 新玩法需求不会被错误写入 `standard-template/Core/SharedCore/`
- 除非用户明确要求维护模板本身，否则不要修改 `standard-template/`
- Unreal 承接目录内已有可引用 `GamePackage/` 的托管宿主工程
- 模板残留测试、模板运行时名称和模板迁移文档不会继续被当作当前游戏产物使用

## 推荐内容

`GamePackage/` 适合放：

- 状态模型
- 命令与事件
- 规则求值器
- UI ViewModel
- 资源逻辑引用键
- Gameplay Spec 导出器

`GamePackage.Tests/` 适合放：

- 单元测试
- 回放测试
- snapshot 对比
- 当前游戏命令和状态流的专属断言
- 迁移一致性基准

## 严格边界

以下内容不应写入 `GamePackage/`：

- Godot 节点路径与节点 API
- Unreal Actor、Component 与 UMG API
- 直接物理求解或宿主生命周期函数
- 编辑器自动化逻辑

这些内容应保留在 Godot Host、Unreal Host 或 `.opencode` 工作流中。