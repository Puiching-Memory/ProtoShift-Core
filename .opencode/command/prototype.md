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
5. 在新增玩法脚本前先执行一次真实编译：`dotnet build workspace/<project-slug>/<assembly-name>.csproj`
6. 将模板扩展为目标玩法场景和脚本
7. 脚本生成或修改后再执行一次真实编译；若失败，优先修工程或代码，而不是先重开编辑器或刷新 UID
8. 在任何打开编辑器或运行项目前，先关闭已有 Godot 编辑器或运行实例，确保只保留一个 Godot 实例
9. 通过 Godot MCP 打开项目并实际运行一次核心玩法流程
10. 读取运行日志；若启动失败、脚本报错或玩法未进入主循环，先修复再运行一次
11. 将共享规则写入 SharedCore
12. 当开发完成后，再先关闭已有 Godot 实例，然后通过 Godot MCP 启动最终版本游戏，交给人类检查；该最终验收启动默认保持运行，不要立刻自动发送停止运行命令，除非用户明确要求结束验收
13. 运行初始测试验证

强制要求：
- 新 Godot 原型优先使用 ProtoShift MCP 的 `initialize_godot_project`，不要从零手写 `project.godot`、`.csproj` 和主入口场景
- 复制模板后必须完成项目命名一致性检查
- 脚本生成前后都必须有真实 `dotnet build` 证据
- 任意时刻只允许一个 Godot 编辑器实例存在；若已经启动过一次，先关闭再继续
- 交付前必须启动最终版本游戏给人类检查，且默认保持运行直到用户验收结束或另有明确指示
- 不要跳过 Godot 版本检查
- 不要在“文件已生成”时提前结束
- 默认必须完成一次 Godot MCP 运行验证闭环，除非 MCP 或运行环境不可用
- 若未能完成运行验证，必须明确说明阻塞点和未验证部分

用户描述：$ARGUMENTS
