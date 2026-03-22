---
name: unreal-actor
description: "Unreal Actor 管理技能：通过 Remote Control / UnrealSharp 操作 Actor 和 Component"
---

# Unreal Actor 管理

## 能力

- 通过 Remote Control API 生成 Actor
- 设置 Actor / Component 属性
- 查询关卡中的对象
- 通过 UnrealSharp 加载项目级 `GamePackage`
- 管理 Blueprint 引用
- 将迁移产物映射为 Unreal 宿主结构
- 校对 Unreal 承接结果是否仍然消费当前游戏的真实 `GamePackage`、UiSchema 和输入契约

## Remote Control API 模式

### 调用函数

```http
PUT http://127.0.0.1:30010/remote/object/call
{
  "objectPath": "/Script/Engine.Default__KismetSystemLibrary",
  "functionName": "PrintString",
  "parameters": { "InString": "Hello" }
}
```

### 读写属性

```http
PUT http://127.0.0.1:30010/remote/object/property
{
  "objectPath": "/Game/Maps/Main.Main:PersistentLevel.PlayerStart",
  "propertyName": "RelativeLocation"
}
```

## UnrealSharp 集成

- `GamePackage` → 作为 UnrealSharp 托管项目引用或编译产物加载
- 引用方式：在 UnrealSharp C# 项目中添加 `ProjectReference` 或构建产物引用
- Unreal Host 负责把宿主输入、物理和 UI 事件转发给 `GamePackage`
- 不把具体玩法规则改写进 Actor / Blueprint，除非它明确属于 `host-only backlog`
- Unreal 侧工作默认发生在持续演进的同一项目中，不等待一个单独的“迁移阶段”才开始考虑承接

## Actor 类型映射（Godot → Unreal）

| Godot | Unreal |
|-------|--------|
| Node3D | AActor |
| CharacterBody3D | ACharacter |
| RigidBody3D | AActor + UPrimitiveComponent (Simulate Physics) |
| Area3D | ATriggerBox / ATriggerSphere |
| Timer | FTimerManager |
