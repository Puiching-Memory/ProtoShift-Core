---
name: godot-scene
description: "Godot 场景管理技能：创建、修改和查询 Godot 场景结构"
---

# Godot 场景管理

## 能力

- 获取当前 Godot 版本并确认运行环境
- 创建新场景（2D / 3D / UI）
- 添加、删除、移动节点
- 设置节点属性
- 连接信号
- 管理资源引用
- 打开并运行 Godot 项目进行一次最小玩法验证
- 读取运行日志并定位启动/场景/脚本问题
- 维护 Godot Host 壳层与 `GamePackage` 的接线边界
- 在做 Godot 侧修改时，同步维护 Unreal 承接所需的节点映射、输入契约和文档一致性

## 约束

- 场景文件使用 `.tscn` 格式
- 节点路径使用 `/` 分隔
- 根节点必须有明确类型
- C# 脚本附加到节点时使用 `[Tool]` 属性标记编辑器可见部分
- 初始化新 Godot 原型时，优先调用 ProtoShift MCP 的 `initialize_godot_project` 工具；仅在该工具不可用时，才手动复制 `standard-template/`
- 除非用户明确要求维护模板本身，否则不要修改 `standard-template/`，只修改 `workspace/<project-slug>/`
- 模板初始化后，必须确认 `.csproj` 文件名、`AssemblyName`、`RootNamespace` 和 `project.godot` 的 `project/assembly_name` 已统一替换
- 模板初始化后，必须确认以下目录全部存在：`GamePackage/`、`GamePackage.Tests/`、`GodotHost/`、`Migration/`、`UnrealProject/`
- 具体玩法规则必须优先写入 `workspace/<project-slug>/GamePackage/`，Godot 脚本只保留宿主逻辑
- Godot 宿主脚本默认落在 `GodotHost/Scripts/`，不要把新宿主脚本继续写回 `scripts/`
- 不要把 Godot 视为独立阶段；Godot 侧场景结构、命名和输入设计必须默认可被 Unreal 承接复用或映射
- 在新增或修改任何 C# 脚本前后，都要各执行一次真实编译：优先 `dotnet build workspace/<project-slug>/<assembly-name>.csproj`
- 任何再次打开编辑器、重新打开项目或重新运行项目前，先关闭已有的 Godot 编辑器或运行实例，确保同一时刻只有一个 Godot 实例
- 完成场景或脚本生成后，不能默认结束；应继续通过 Godot MCP 打开并运行项目一次

## 模板审计清单

在开始生成玩法前，先逐项确认：

1. 主 `.csproj` 已改名且命名空间正确
2. `GamePackage/` 存在并可编译
3. `GamePackage.Tests/` 存在并可运行
4. `GodotHost/Scripts/Main.cs` 已作为主宿主脚本接到场景
5. `Migration/` 内已有 `scene-manifest.json`、`gameplay-spec.json`、`host-only-backlog.json`
6. `UnrealProject/Managed/` 内已存在托管宿主工程与协调器文件
7. README 与 `Migration/` 说明不会继续保留模板语义

## 验证闭环

1. 用 Godot MCP 获取当前 Godot 版本，确认本次运行环境
2. 若之前启动过 Godot，先关闭已有编辑器或运行实例
3. 用 Godot MCP 打开 `workspace/<project-slug>/project.godot`
4. 运行项目，进入主场景或核心玩法入口
5. 读取 debug 输出和错误日志
6. 若日志报错或玩法主循环未启动，先修复再重复运行
7. 停止运行并向用户报告已验证结果、残留问题和下一步

## 最终交付检查

- 当你判断玩法开发已经完成，不要只给出文字说明
- 必须先关闭之前的 Godot 实例，再通过 Godot MCP 启动最终版本游戏一次，让人类可以直接检查当前成品；这一步默认保持运行，不自动发送停止运行命令，除非用户验收结束或另有明确指示
- 最终交付检查以“给人类看最终版本”为目的，不再把它当作一次普通的后台自检运行

## Godot C# 故障排查

- 当日志出现 `Cannot instantiate C# script because the associated class could not be found` 时，不要默认归因于“类名和文件名不匹配”
- 先把它当作“C# 工程没有成功编译”处理，并按下面顺序排查：
	1. 确认项目根目录存在 `.csproj`
	2. `.csproj` 的 `Sdk` 必须显式带上与当前 Godot Mono 版本匹配的版本号，例如 `Godot.NET.Sdk/4.6.1`
	3. `project.godot` 的 `config/features` 应包含 `C#`
	4. `project/assembly_name`、`.csproj` 的 `AssemblyName` 和 `RootNamespace` 保持一致
	5. 对项目执行一次真实编译，优先 `dotnet build workspace/<project-slug>/<project-name>.csproj`
	6. 只有在编译成功后，才继续用 Godot MCP 重新运行并检查日志
- 若 `protoshift_compile` 没有返回真实编译结果，或工具本身报错，不要把它当成已完成编译；必须回退到真实的 `dotnet build` 或 Godot 的 C# 构建能力
- 若真实编译失败，优先根据编译器错误修复脚本、工程文件或 SDK 版本，不要在没有编译证据的情况下反复修改类名、文件名或场景引用
- 若模板工程中多个脚本同时报 `Cannot instantiate`，优先怀疑“程序集整体未被正确加载”，不要先查 `.uid`、缓存、资源重存或单个场景路径
- 若 `.godot/mono/temp/bin/Debug/` 下仍出现模板默认程序集名，而项目配置已经改为新名字，说明模板初始化或命名替换不完整；先统一工程命名，再重新 `dotnet build`
- `godot_get_debug_output` 一次返回空错误并不代表问题已解决；至少要有一次真实编译成功，随后再取一次稳定日志确认

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
