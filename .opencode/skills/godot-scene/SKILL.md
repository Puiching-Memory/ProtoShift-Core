---
name: godot-scene
description: "Godot 场景管理技能：创建、修改和查询 Godot 场景结构"
---

# Godot 场景管理

## 能力

- 创建新场景（2D / 3D / UI）
- 添加、删除、移动节点
- 设置节点属性
- 连接信号
- 管理资源引用

## 约束

- 场景文件使用 `.tscn` 格式
- 节点路径使用 `/` 分隔
- 根节点必须有明确类型
- C# 脚本附加到节点时使用 `[Tool]` 属性标记编辑器可见部分

## 场景模板

### 2D 场景

```
[gd_scene format=3]
[node name="Root" type="Node2D"]
```

### 3D 场景

```
[gd_scene format=3]
[node name="Root" type="Node3D"]
```

### UI 场景

```
[gd_scene format=3]
[node name="Root" type="Control"]
```

## 节点类型映射（Godot → Unreal）

| Godot | Unreal |
|-------|--------|
| Node3D | Actor |
| CharacterBody3D | Character |
| RigidBody3D | Physics Actor |
| MeshInstance3D | Static Mesh Component |
| Camera3D | Camera Actor |
| DirectionalLight3D | Directional Light |
| Control | UMG Widget |
