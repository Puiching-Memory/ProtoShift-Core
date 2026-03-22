# 快速开始

## 前置条件

- .NET 8 SDK
- Godot 4.x（带 .NET 支持）
- opencode CLI（通过 submodule 集成）
- （可选）Unreal Engine 5 + UnrealSharp 插件

## 构建

```bash
# 克隆仓库（含 submodule）
git clone --recurse-submodules https://github.com/Puiching-Memory/ProtoShift-Core.git
cd ProtoShift-Core

# 构建 C# 解决方案
dotnet build ProtoShift.sln
```

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
- 默认闭环应为：先获取 Godot 当前版本 → 用 ProtoShift MCP 的 `initialize_godot_project` 初始化 `workspace/<project-slug>/` → 先做一次 `dotnet build` → 按需求修改场景/脚本 → 再做一次 `dotnet build` → 若之前已开过 Godot，先关闭旧实例 → 用 Godot MCP 打开项目 → 运行一次 → 读取日志 → 必要时修复并重跑 → 交付前再关闭旧实例并启动最终版本给人类检查
- 只有在 MCP 未连接、Godot 路径无效或本机环境不可运行时，才允许退化为“仅生成代码并报告阻塞”

## 第一个原型

在 opencode shell 中输入：

```
/prototype 一个简单的 2D 弹球游戏
```

Agent 会：
1. 在 `workspace/<project-slug>/` 下创建 Godot 项目
2. 生成弹球场景和物理脚本
3. 将碰撞规则写入 SharedCore
4. 运行初始测试

## 迁移到 Unreal

```
/migrate 将弹球原型迁移到 Unreal 5
```

Agent 会：
1. 导出 Godot 项目的迁移模型
2. 分析 SharedCore 共享逻辑覆盖率
3. 在 Unreal 中生成对应的 Actor/Component 结构
4. 通过 UnrealSharp 引用 SharedCore
5. 运行验证测试
