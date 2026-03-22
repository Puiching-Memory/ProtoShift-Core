---
name: migrate
description: "将 Godot 原型迁移到 Unreal 5"
agent: protoshift
---

将当前 Godot 原型导出为迁移模型，并在 Unreal 5 中生成对应结构。

步骤：
1. 分析当前 Godot 项目结构
2. 导出迁移模型（场景树、脚本引用、资源清单）
3. 列出 SharedCore 已有共享逻辑
4. 在 Unreal 侧通过 UnrealSharp 接入 SharedCore
5. 生成 Actor/Component 结构对应 Godot Node 树
6. 映射 UiSchema 到 UMG
7. 运行验证测试

目标：$ARGUMENTS
