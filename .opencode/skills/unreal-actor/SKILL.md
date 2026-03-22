---
name: unreal-actor
description: "Unreal Actor 管理技能：通过 Remote Control / UnrealSharp 操作 Actor 和 Component"
---

# Unreal Actor 管理

## 能力

- 通过 Remote Control API 生成 Actor
- 设置 Actor / Component 属性
- 查询关卡中的对象
- 通过 UnrealSharp 执行 C# 逻辑
- 管理 Blueprint 引用

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

- SharedCore DLL → 放入 `Managed/` 目录
- 引用方式：在 UnrealSharp C# 项目中添加 ProjectReference
- 调用 SharedCore API 与在 Godot C# 中完全一致

## Actor 类型映射（Godot → Unreal）

| Godot | Unreal |
|-------|--------|
| Node3D | AActor |
| CharacterBody3D | ACharacter |
| RigidBody3D | AActor + UPrimitiveComponent (Simulate Physics) |
| Area3D | ATriggerBox / ATriggerSphere |
| Timer | FTimerManager |
