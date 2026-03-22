# Godot Standard Template

这个模板用于 ProtoShift 的新 Godot 原型初始化。

目标：

- 降低普通模型从零创建 Godot C# 项目时的出错概率
- 提供 Godot 与 Unreal 5 可共同承接的统一项目骨架
- 让 Agent 从第一天起就把玩法写进项目级 `GamePackage`，而不是写死在 Godot 宿主脚本里

## 使用方式

1. 将整个目录复制到 `workspace/<project-slug>/`
2. 将 `ProtoShiftGame.csproj` 重命名为目标项目名，例如 `space-shooter.csproj`
3. 替换以下文本：
   - `ProtoShiftGame` -> 目标展示名或程序集名
   - `res://scenes/Main.tscn` 保持为主入口，除非你明确切换主场景
4. 确认已生成 `GamePackage/`、`GamePackage.Tests/`、`GodotHost/`、`Migration/`、`UnrealProject/`
5. 再开始新增玩法场景、脚本和资源

## 打开方式

- 模板根目录本身就是 Godot 项目，可直接打开 `project.godot`
- `UnrealProject/` 内提供 Unreal 5 项目入口，可直接打开 `ProtoShiftGame.uproject`
- Unreal 首次打开后产生的本机目录和生成文件不会保存在模板里

## 目录说明

- `GamePackage/`：项目级可移植玩法包，禁止引用 `Godot.*` 或 `UnrealSharp.*`
- `GamePackage.Tests/`：纯 .NET 测试和 snapshot 基线
- `Core/`：模板内共享 core，包含 SharedCore、EngineBridge、GodotBridge、UnrealBridge、UiSchema
- `GodotHost/`：Godot 宿主脚本，只做输入、物理、场景和 UI 接线
- `Migration/`：`Scene Manifest`、`Gameplay Spec`、UiSchema、snapshot 和 backlog
- `UnrealProject/`：UnrealSharp 承接工程骨架，包含托管宿主项目与绑定配置
- `scenes/`：Godot 场景入口与子场景
- `assets/`：贴图、音频、材质等资源
- `data/`：配置与原型数据

## 模板约束

- 不要删除 `project.godot`、主 `.csproj`、`GamePackage/`、`GodotHost/`、`Migration/`
- 优先在这个骨架上增量修改，而不是重新 new 一个 Godot 项目
- 具体玩法规则默认写入 `GamePackage/`
- Godot 宿主脚本默认写入 `GodotHost/Scripts/`
- Unreal 承接宿主脚本默认写入 `UnrealProject/Managed/`
- 若需要 2D 或 3D 专用根场景，在 `Main` 下挂接新的子场景即可
- 该目录是模板源，普通原型任务应复制到 `workspace/` 后修改，不要直接改这里
- `.godot/`、各层 `bin/` / `obj/`、以及 `UnrealProject/Binaries/`、`DerivedDataCache/`、`Intermediate/`、`Saved/`、`Script/` 等目录属于本机生成物，不应进入模板
- Unreal 输入和 Widget 绑定以 `UnrealProject/Config/input-actions.json`、`UnrealProject/Config/widget-bindings.json` 为模板契约；其余引擎默认配置应尽量保持最小

## 关键文件

- `GamePackage/PrototypeRuntime.cs`：项目级运行时入口
- `GamePackage/PrototypeCommands.cs`：输入命令定义
- `GamePackage/PrototypeEvents.cs`：领域事件定义
- `GodotHost/Scripts/Main.cs`：Godot 宿主编排入口
- `GodotHost/Scripts/TemplateHudPresenter.cs`：HUD 绑定层
- `UnrealProject/Managed/ProtoShiftGame.UnrealHost.csproj`：Unreal 承接托管工程
- `UnrealProject/Managed/TemplateUnrealHostCoordinator.cs`：Unreal 宿主编排骨架
- `Migration/gameplay-spec.json`：运行时、命令、事件、视图模型合约