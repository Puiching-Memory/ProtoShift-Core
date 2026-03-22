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
# 进入 opencode 目录并启动
cd opencode
# 按 opencode 文档安装和启动
```

opencode 启动后会自动加载：

- `.opencode/opencode.jsonc` — 主配置
- `AGENTS.md` — 系统指令
- `.opencode/agent/*.md` — Agent 定义
- `.opencode/command/*.md` — 命令定义
- `.opencode/skills/*/SKILL.md` — 技能定义
- `plugins/protoshift.ts` — 插件（hooks + tools）

## 第一个原型

在 opencode shell 中输入：

```
/prototype 一个简单的 2D 弹球游戏
```

Agent 会：
1. 创建 Godot 项目
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
