# Godot CLI 工作流

这份文件是路由启发，不是固定流程图。

它的作用是帮助 agent 快速落到稳定起点、减少参数乱试；如果当前上下文已经给出更好的路径，可以直接偏离这里的建议。

## 按能力面思考，不要按参数表思考

抽象 Godot CLI 时，先暴露稳定的任务能力面，再把每个任务映射到最窄、最稳定的命令形态。

常见能力分类：

- 环境发现
  - 目标：确认可执行文件、版本和项目根目录。
  - 常见形态：`--version`，再配合文件系统检查 `project.godot`。
- 打开或修复项目
  - 目标：进入编辑器、项目管理器或恢复模式。
  - 常见形态：`-e`、`-p`、`--recovery-mode`、`--path`。
- 运行并观察
  - 目标：启动默认场景或指定场景，并采集运行表现。
  - 常见形态：`-d --path <project> [--scene <scene>]`，再配合 stdout、stderr 或 `--log-file`。
- 自动化引擎内修改
  - 目标：创建场景、添加节点、设置资源、重存资源，或执行其他更适合在引擎内部完成的操作。
  - 常见形态：`--headless --path <project> --script <tool-script.gd> <payload>`。
- 渲染输出或截图
  - 目标：以场景真实渲染结果生成截图或其他帧输出。
  - 常见形态：`--path <project> --script <tool_capture_scene.gd> <payload>`。
- 初始化全新项目
  - 目标：在空目录中生成 `project.godot`、基础目录和可选的主场景。
  - 常见形态：`--headless --script <tool_init_project.gd> <payload>`。
- 导入、构建和导出
  - 目标：刷新导入、编译 C# 解决方案或产出发布物。
  - 常见形态：`--import`、`--build-solutions`、`--export-debug`、`--export-release`、`--export-pack`、`--export-patch`。
- 启动或渲染问题诊断
  - 目标：收集日志、做性能观测，或绕过出问题的图形后端。
  - 常见形态：`-v`、`--log-file`、`--print-fps`、`--gpu-validation`、`--remote-debug`、`--display-driver`、`--rendering-driver` 或 `--headless`。

## 先把请求归一化，再拼命令

在动 CLI 之前，先把用户请求压成一个小模型：

- `godot_path`
- `project_path`
- `scene_path`
- `script_path`
- `payload`
- `output_path`
- `preset`
- `diagnostic_flags`

排障时尽量不要在同一次重试里混入无关问题。命令失败时，每次只改一个轴，通常更容易定位原因。

## 单功能脚本是常见稳妥路径

对可重复的引擎内修改，单功能脚本 + 共享 helper 往往是很稳的默认路径，但不是唯一选择。

1. 每个脚本通常只负责一个稳定操作，例如 `tools/tool_create_scene.gd`。
2. 每个脚本通常只接收一个 JSON payload，减少入口分支。
3. 公共逻辑可以放到共享 helper，例如 `tools/tool_common.gd`。
4. 单功能脚本本身尽量只保留自身必需的最小业务逻辑。

这种模式更像一组可复用工具，便于按需复制、按需组合，也更容易在失败时缩小排查范围。

- 大多数资源修改脚本使用 `--headless --script`。
- 场景截图这类依赖渲染结果的脚本改用普通 `--script`。
- 初始化项目脚本是另一个特殊情况：它不依赖现有项目，也不需要 `--path`。

如果当前任务更适合统一入口、一次性脚本，或更高层封装，也可以直接这样做。

## 用简单决策树选择模式

通常可以按下面的方式路由：

- 需要可见的编辑器会话：用 `-e` 或 `-p`。
- 需要运行游戏或某个场景：用 `-d` 加 `--path`，再按需加 `--scene`。
- 需要稳定、非交互自动化：优先用 `--headless --script` 加单功能脚本。
- 需要场景截图或其他依赖渲染结果的输出：用普通 `--script`，不要带 `--headless`。
- 需要初始化一个还不存在的 Godot 项目：用 `--headless --script` 直接运行 `tool_init_project.gd`，不要带 `--path`。
- 需要导入、构建或导出：保持以 `--path` 为根，再追加最相关的构建/导出参数。
- 需要诊断：先加详细输出和日志，如果第一轮信息不够，再加后端或专项调试参数。
