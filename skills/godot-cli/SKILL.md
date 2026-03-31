---
name: godot-cli
description: "用 Godot 命令行操作项目。适用于 Codex 需要定位 Godot 可执行文件、验证 `project.godot`、启动编辑器或项目管理器、运行项目或指定场景、执行无头脚本或渲染脚本、导入资源、构建 C# 解决方案、导出预设或资源包、输出场景截图、收集日志、排查启动或运行问题的场景。把 Godot CLI 抽象成面向任务的工作流，用于“运行这个项目”“打开编辑器”“导入后退出”“导出构建”“给某个场景截图”或“排查 Godot CLI 失败”等请求。"
---

# Godot 命令行

把这个 skill 当成导航，不是硬性规范。

- 它的主要作用是提供稳定起点、暴露高价值参数、减少重复试错。
- 它不规定唯一实现路径。上下文已经给出更快、更稳或更适合的方案时，可以直接偏离这里的建议。
- 对一次性验证任务，可以直接用更短的 CLI 或临时脚本；对会重复出现或容易出错的任务，再考虑抽成稳定工具。

## 先判断任务类型

不要一上来堆参数，先判断这次任务更接近哪种执行模式。

1. 需要打开、检查、修复或导入项目时，通常使用编辑器模式。
2. 需要启动游戏或指定场景时，通常使用运行模式。
3. 需要稳定自动化或在引擎内部执行修改时，通常先看脚本模式。
4. 需要导入、构建 C# 或导出产物时，通常使用导入/构建/导出模式。
5. 需要日志、后端切换、调试可视化或启动排障时，通常使用诊断模式。
6. 需要从零生成一个全新项目目录时，使用初始化项目脚本。
7. 需要输出某个场景的渲染截图时，使用场景截图脚本。

需要查看 CLI 工作流的高层映射时，读 [references/cli-workflows.md](references/cli-workflows.md)。
需要把脚本模式拆成引擎内操作能力时，先读 [references/operation-capabilities.md](references/operation-capabilities.md)。
需要确认各操作的参数契约和调用形态时，读 [references/operation-contracts.md](references/operation-contracts.md)。
需要直接拷贝最小功能脚本时，通常从 [assets/tool_common.gd](assets/tool_common.gd) 和同目录下的 `tool_*.gd` 开始。
需要初始化全新项目时，使用 [assets/tool_init_project.gd](assets/tool_init_project.gd)。
需要输出场景截图时，使用 [assets/tool_capture_scene.gd](assets/tool_capture_scene.gd)。
需要具体的 PowerShell 命令模板或参数选型时，读 [references/cli-recipes.md](references/cli-recipes.md)。

## 先补齐基础信息

在拼命令前，通常先确认这些输入：

- 识别 Godot 可执行文件路径。用户已经给出时，优先原样复用。
- 确认项目目录中存在 `project.godot`。
- 大多数项目命令都会以 `--path <project-dir>` 为锚点。
- 已有 `--path` 时，常见起点是 `scenes/Main.tscn` 或 `tools/check_project.gd` 这样的项目内相对路径。
- 不要把本机绝对路径写进文档、模板或持久化配置。示例里统一使用占位符或项目内相对路径，实际执行时再从当前上下文推导。
- “初始化全新项目”是常见例外：它的 `target_dir` 指向目标项目目录本身，不依赖现有 `project.godot`。

## 先拼最小可用命令

通常可以先从最小命令骨架开始，再只追加和目标直接相关的一组参数。

常见基础形态：

- `editor`：`"<godot>" -e --path "<project>"`
- `run`：`"<godot>" -d --path "<project>"`
- `headless-script`：`"<godot>" --headless --path "<project>" --script "<script>"`
- `render-script`：`"<godot>" --path "<project>" --script "<script>"`
- `import-build-export`：`"<godot>" --path "<project>" --import --quit`
- `diagnose`：`"<godot>" -v --path "<project>" --log-file "<log>"`

然后再按任务追加真正改变行为的参数，比如 `--scene`、`--quit`、`--quit-after`、`--check-only`、`--display-driver`、`--audio-driver`、`--rendering-driver`、`--export-*` 或 `--build-solutions`。

## 优先复用稳定的命令抽象

对会重复出现或容易出错的引擎内修改，通常值得抽成稳定操作；如果只是一次性验证，直接命令或临时脚本也可以。

- 常见做法是把稳定操作写成单功能 GDScript，再根据是否依赖渲染决定走 `--headless --script` 还是普通 `--script`。
- 把路径规范化、场景加载/保存、节点定位、类型实例化这类公共逻辑抽到共享 helper。
- 让每个单功能脚本只接收一个 JSON payload，降低拼接和排障成本。
- 共享 helper 和初始化脚本都支持直接传 JSON，也支持传 `@payload.json`。
- 把 stdout 和 stderr 作为下一轮迭代的反馈依据。
- 如果统一入口确实更适合当前任务，也可以封装更高层 dispatcher，不必被单功能脚本模式反向束缚。
- 初始化项目脚本是少数可以不带 `--path` 运行的脚本，因为它本身就是用来生成 `project.godot` 的。
- 场景截图这类依赖真实渲染结果的脚本，不要走 `--headless`，应使用普通显示驱动和 `--script`。

当任务属于“创建场景”“添加节点”“加载贴图”“保存场景变体”“修复 UID 引用”“渲染场景截图”这类操作时，通常可以先看是否已有对应的单功能脚本，而不是从零重新试一轮。

需要扩展或复用这类操作时，先看能力分组，再对照参数契约，最后从 `tool_*.gd` 中挑出最接近的最小脚本。

## 运行、观察、迭代

- 偏验证用途的运行，通常会优先使用 `--quit`、`--quit-after <n>` 或 `--check-only`。
- 非交互自动化、类 CI 检查、导入或资源修改，通常会优先使用 `--headless`。
- 依赖真实渲染结果的任务，例如场景截图，通常优先使用普通 `--script`。
- 在加很多专项调试参数之前，先加 `--verbose` 或 `--log-file`，通常更容易看清问题。
- 排障时尽量每次只改变一个维度：执行模式、场景、渲染器、显示驱动或调试选项。
- 当前行为不明确时，可以显式切换到 `--editor`、`--project-manager`、`--headless` 或普通 `--script`。

## 处理常见边界情况

- 缺少 `project.godot` 时，通常先按项目路径错误处理。
- 导出失败时，常见检查项是预设名和输出路径。预设通常需要存在于 `export_presets.cfg` 中，目标目录也要先存在。
- C# 构建问题通常值得补上 `--build-solutions` 再试一次。
- 渲染或 GPU 启动失败时，常见替代路径是切换显示驱动、渲染驱动，或改用 `--headless`。
- 编辑器启动崩溃时，通常可以先试 `--recovery-mode`。
- 场景截图为空白时，先检查是否误用了 `--headless`、是否缺少可用相机，或是否需要增加 `wait_frames`。
