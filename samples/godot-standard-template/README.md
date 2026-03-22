# Godot Standard Template

这个模板用于 ProtoShift 的新 Godot 原型初始化。

目标：

- 降低普通模型从零创建 Godot C# 项目时的出错概率
- 提供一个对 2D / 3D / UI / 混合原型都可扩展的中性骨架
- 让 Agent 先复制模板，再按需求增量修改，而不是重新拼装工程文件

## 使用方式

1. 将整个目录复制到 `workspace/<project-slug>/`
2. 将 `ProtoShiftGame.csproj` 重命名为目标项目名，例如 `space-shooter.csproj`
3. 替换以下文本：
   - `ProtoShiftGame` -> 目标展示名或程序集名
   - `res://scenes/Main.tscn` 保持为主入口，除非你明确切换主场景
4. 再开始新增玩法场景、脚本和资源

## 目录说明

- `scenes/`：场景入口与子场景
- `scripts/Game/`：玩法逻辑
- `scripts/UI/`：UI 脚本
- `scripts/Systems/`：存档、流程、生成器等系统脚本
- `assets/`：贴图、音频、材质等资源
- `data/`：配置与原型数据

## 模板约束

- 不要删除 `project.godot`、主 `.csproj`、`scenes/Main.tscn`、`scripts/Main.cs`
- 优先在这个骨架上改，而不是重新 new 一个 Godot 项目
- 若需要 2D 或 3D 专用根场景，在 `Main` 下挂接新的子场景即可