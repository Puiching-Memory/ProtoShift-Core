---
name: prototype
description: "创建新的游戏原型项目"
agent: protoshift
---

根据用户描述创建一个新的游戏原型。

步骤：
1. 先通过 Godot MCP 获取当前 Godot 版本
2. 分析用户描述的游戏概念
3. 优先调用 ProtoShift MCP 的 `initialize_godot_project` 初始化 `workspace/<project-slug>/`
4. 初始化后立刻检查 `.csproj` 文件名、`AssemblyName`、`RootNamespace` 与 `project.godot` 的 `project/assembly_name` 是否统一
5. 立刻检查模板目录是否完整：`GamePackage/`、`GamePackage.Tests/`、`GodotHost/`、`Migration/`、`UnrealProject/`
6. 在新增玩法脚本前先执行一次真实编译：`dotnet build workspace/<project-slug>/<assembly-name>.csproj`
7. 从第一天起同时按 Godot 宿主和 Unreal 宿主承接来设计接口，确保同一个 `GamePackage` 是双宿主共享的唯一玩法真相
8. 将模板扩展为 Godot Host 壳层，并把玩法规则、命令、事件、ViewModel 优先写入 `GamePackage/`
9. 为当前新游戏建立或更新 `GamePackage.Tests/` 中的专属测试、snapshot 或 replay，用当前玩法命名替换模板样例语义
10. 同步更新项目 `README`、`Migration/gameplay-spec.json`、`Migration/scene-manifest.json`、UiSchema、输入映射与相关宿主说明，不要等到后续承接校对时才补文档
11. 脚本生成或修改后再执行一次真实编译；若失败，优先修工程或代码，而不是先重开编辑器或刷新 UID
12. 在任何打开编辑器或运行项目前，先关闭已有 Godot 编辑器或运行实例，确保只保留一个 Godot 实例
13. 通过 Godot MCP 打开项目并实际运行一次核心玩法流程
14. 读取运行日志；若启动失败、脚本报错或玩法未进入主循环，先修复再运行一次
15. 校验 Unreal 承接骨架、Migration 产物和项目文档仍然与当前 `GamePackage` 契约一致
16. 当开发完成后，再先关闭已有 Godot 实例，然后通过 Godot MCP 启动最终版本游戏，交给人类检查；该最终验收启动默认保持运行，不要立刻自动发送停止运行命令，除非用户明确要求结束验收
17. 运行当前游戏的初始测试验证

强制要求：
- 新 Godot 原型优先使用 ProtoShift MCP 的 `initialize_godot_project`，不要从零手写 `project.godot`、`.csproj` 和主入口场景
- 模板初始化后若缺少 `GamePackage/`、`GamePackage.Tests/`、`GodotHost/`、`Migration/` 或 `UnrealProject/`，先补齐结构，再继续生成玩法代码
- 模板初始化后若缺少 `GamePackage/PrototypeRuntime.cs`、`GodotHost/Scripts/Main.cs` 或 `UnrealProject/Managed/*.csproj`，先修复模板骨架，再继续生成玩法代码
- 不要把 Unreal 承接理解成后置状态切换；从原型开始就要维护 `GamePackage`、测试、Migration 与 Unreal 承接契约的一致性
- 不要把工作拆成“先交给 Godot agent，再切 Unreal agent”的串行模式；主 Agent 需要始终按双宿主目标规划和检查输出
- 具体游戏规则默认写入 `workspace/<project-slug>/GamePackage/`，不要写入 `standard-template/Core/SharedCore/`
- 除非用户明确要求维护模板本身，否则不要修改 `standard-template/`，只修改初始化出来的 `workspace/<project-slug>/`
- Godot 宿主脚本默认写入 `workspace/<project-slug>/GodotHost/`，不要把宿主逻辑继续堆进 `scripts/`
- 复制模板后必须完成项目命名一致性检查
- 脚本生成前后都必须有真实 `dotnet build` 证据
- 必须为当前新游戏创建或更新 `GamePackage.Tests/`，不要只保留模板测试名、模板快照和模板运行时断言
- 每次新增或修改玩法契约后，都要同步更新该项目文档与迁移产物，避免 README、UiSchema、Gameplay Spec、输入映射落后于代码
- 任意时刻只允许一个 Godot 编辑器实例存在；若已经启动过一次，先关闭再继续
- 交付前必须启动最终版本游戏给人类检查，且默认保持运行直到用户验收结束或另有明确指示
- 不要跳过 Godot 版本检查
- 不要在“文件已生成”时提前结束
- 默认必须完成一次 Godot MCP 运行验证闭环，除非 MCP 或运行环境不可用
- 若未能完成运行验证，必须明确说明阻塞点和未验证部分

用户描述：$ARGUMENTS
