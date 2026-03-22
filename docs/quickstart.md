# 快速开始

## 前置条件

- .NET 8 SDK
- Godot 4.x（带 .NET 支持）
- opencode CLI（通过 submodule 集成）
- （可选）Unreal Engine 5 + UnrealSharp 插件 + unreal-mcp

## 构建

```bash
# 克隆仓库（含 submodule）
git clone --recurse-submodules https://github.com/Puiching-Memory/ProtoShift-Core.git
cd ProtoShift-Core

# 构建 C# 解决方案
dotnet build ProtoShift.sln
```

`ProtoShift.sln` 现在直接包含 `standard-template/` 下的 Godot 主工程、`GamePackage`、`GamePackage.Tests`、`UnrealHost` 与 `Core/` 五个通用项目，便于在一个模板里维护跨引擎全局视图。

普通原型任务不应直接修改 `standard-template/`；应先复制到 `workspace/<project-slug>/`，之后只在工作区副本上开发。

## 运行 opencode

```bash
# 在仓库根目录启动 opencode
opencode
```

opencode 启动后会自动加载：

- `.opencode/opencode.jsonc` — 主配置
- `AGENTS.md` — 系统指令
- `.opencode/agent/*.md` — Agent 定义
- `.opencode/command/*.md` — 命令定义
- `.opencode/skills/*/SKILL.md` — 技能定义
- `.opencode/plugins/protoshift.ts` — 插件入口（自动发现）

## MCP 配置

- Godot MCP 使用 `npx -y @coding-solo/godot-mcp`
- ProtoShift MCP 使用 `node .opencode/mcp/protoshift-mcp.mjs`
- 通过 `GODOT_PATH` 指向本机 Godot 可执行文件
- 不要直接把 Godot 可执行文件作为 MCP server 命令，否则可能出现编辑器 UI 被拉起后自行退出
- Unreal MCP 使用 GenOrca/unreal-mcp，自行安装插件后，把 `UNREAL_MCP_SERVER_DIR` 指向其中的 `mcp-server` 目录

## Agent 与 Subagent

- `protoshift` 设为默认主 Agent
- `godot` 与 `unreal` 是 subagent，不会像主 Agent 一样通过 Tab 切换
- 主 Agent 会在任务聚焦单一引擎时自动委派，也可以显式使用 `@godot` 或 `@unreal`

## Godot 原型默认闭环

- 当用户直接说“制作一个飞机大战游戏”这类自然语言需求时，ProtoShift 不应只写文件
- 默认闭环应为：先获取 Godot 当前版本 → 用 ProtoShift MCP 的 `initialize_godot_project` 初始化 `workspace/<project-slug>/` → 审计模板目录 `GamePackage/`、`GamePackage.Tests/`、`GodotHost/`、`Migration/`、`UnrealProject/` → 先做一次 `dotnet build` 和一次 `dotnet test` → 按需求修改 `GamePackage` 与 Godot 宿主壳层 → 再做一次 `dotnet build` → 若之前已开过 Godot，先关闭旧实例 → 用 Godot MCP 打开项目 → 运行一次 → 读取日志 → 必要时修复并重跑 → 停止本次开发期验证运行并汇报结果 → 交付前再关闭旧实例并启动最终版本给人类检查，且最终验收启动后默认保持运行
- 只有在 MCP 未连接、Godot 路径无效或本机环境不可运行时，才允许退化为“仅生成代码并报告阻塞”

## 第一个原型

在 opencode shell 中输入：

```
/prototype 一个简单的 2D 弹球游戏
```

Agent 会：
1. 在 `workspace/<project-slug>/` 下创建 Godot 项目
2. 审计 `GamePackage/`、`GamePackage.Tests/`、`GodotHost/`、`Migration/`、`UnrealProject/` 模板结构
3. 将玩法规则写入项目级 `GamePackage/`
4. 运行初始测试与 Godot 验证闭环

## 迁移到 Unreal

```
/migrate 将弹球原型迁移到 Unreal 5
```

Agent 会：
1. 导出 `Scene Manifest`、`Gameplay Spec`、`UiSchema/ViewModel` 与 `host-only backlog`
2. 用 unreal-mcp 创建 Unreal 项目骨架、输入映射和基础对象结构
3. 通过 UnrealSharp 加载同一个 `GamePackage`
4. 将 Manifest 映射为 Actor/Component，将 UiSchema 映射到 UMG
5. 对比 gameplay snapshot 并运行验证测试
