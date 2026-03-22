# 8-Ball Pool 迁移样例

本目录包含一个最小的迁移模型样例，演示从 Godot 原型到 Unreal 承接的数据格式。

## 文件

- `8ball-migration.json` — 迁移模型 JSON

## 迁移模型说明

该模型描述了一个简单的台球游戏原型：

- **场景结构**：台面（StaticBody3D）→ 球袋（Area3D）→ 白球（RigidBody3D）
- **脚本分类**：SharedCore 中的状态机和任务系统可直接复用，GDScript 部分需在 Unreal 侧重写
- **资源清单**：GLTF 格式的网格资源，可跨引擎使用
- **UI 引用**：HUD 使用 UiSchema 定义，可双端映射

## 映射关系

| Godot 节点 | Unreal 对应 |
|------------|-------------|
| StaticBody3D (Table) | AStaticMeshActor |
| Area3D (Pocket) | ATriggerSphere |
| RigidBody3D (Ball) | AActor + UStaticMeshComponent (Simulate Physics) |
| FiniteStateMachine (SharedCore) | 通过 UnrealSharp 直接调用 |
