# Godot 命令行配方

把这些偏 PowerShell 的模板当作起点，不要把它们当成唯一答案。

用户已经提供 Godot 可执行文件路径时，优先直接复用。任务更简单时可以直接缩减，任务更复杂时也可以按当前上下文重组这些模板。

```powershell
$godot = '<godot-executable>'
$project = '<project-directory>'
```

## 检查环境

获取引擎版本：

```powershell
& $godot --version
```

在执行 CLI 命令前，先确认目录确实是 Godot 项目：

- 检查 `$project\project.godot` 是否存在。
- 除非任务明确要求传 `project.godot` 文件本身，否则通常优先使用 `--path $project`。

## 编辑器相关工作流

为某个项目打开编辑器：

```powershell
& $godot -e --path $project
```

显式打开项目管理器：

```powershell
& $godot -p
```

以恢复模式启动编辑器：

```powershell
& $godot -e --recovery-mode --path $project
```

导入资源后退出：

```powershell
& $godot --path $project --import --quit
```

构建 C# 解决方案后退出：

```powershell
& $godot --path $project --build-solutions --quit
```

## 运行和调试游戏

以调试模式运行项目默认场景：

```powershell
& $godot -d --path $project
```

运行指定场景：

```powershell
& $godot -d --path $project --scene 'scenes/Main.tscn'
```

把用户参数透传给项目：

```powershell
& $godot --path $project -- --seed 123 --mode benchmark
```

只运行有限帧数：

```powershell
& $godot -d --path $project --quit-after 120
```

## 用脚本做自动化

以无头模式运行脚本：

```powershell
& $godot --headless --path $project --script 'tools/check_project.gd'
```

只做脚本检查并退出，不完整执行：

```powershell
& $godot --headless --path $project --script 'tools/check_project.gd' --check-only
```

运行单功能创建场景脚本：

```powershell
& $godot --headless --path $project --script 'tools/tool_create_scene.gd' '{"scene_path":"scenes/Test.tscn","root_node_type":"Node2D"}'
```

运行单功能重存资源脚本：

```powershell
& $godot --headless --path $project --script 'tools/tool_resave_resources.gd' '{}'
```

复制单功能脚本时，记得把共享助手 `tools/tool_common.gd` 一起带上。

如果是在 Windows 上通过 `Start-Process` 调用，`@payload.json` 往往比内联 JSON 更稳，能减少命令行转义带来的问题。

运行场景截图脚本：

```powershell
$payloadFile = Join-Path $env:TEMP 'godot-capture-payload.json'
@{
  scene_path = 'scenes/Main.tscn'
  output_path = 'artifacts/main-scene.png'
  resolution = @(1280, 720)
  wait_frames = 4
} | ConvertTo-Json -Compress | Set-Content -LiteralPath $payloadFile -Encoding UTF8

& $godot --path $project --script 'tools/tool_capture_scene.gd' "@$payloadFile"
```

场景截图不要带 `--headless`，否则只会落到 dummy 渲染路径，拿不到可信的渲染结果。

如果当前任务更适合纯 CLI 抓帧，也可以改用 `--write-movie`。它更偏录制或抓帧流程，不是默认截图工具，但在某些快速验证场景下也够用。

初始化一个全新项目目录：

```powershell
$target = '<target-directory>'
$payloadFile = Join-Path $env:TEMP 'godot-init-payload.json'
@{
  target_dir = $target
  project_name = 'My Game'
  create_main_scene = $true
  main_scene_path = 'scenes/Main.tscn'
  root_node_type = 'Node2D'
  folders = @('scenes', 'scripts', 'assets')
} | ConvertTo-Json -Compress | Set-Content -LiteralPath $payloadFile -Encoding UTF8

& $godot --headless --script '.\tool_init_project.gd' "@$payloadFile"
```

这一步不要带 `--path`，因为目标目录里此时还没有 `project.godot`。
如果是在 Windows 上通过 `Start-Process` 调用，优先使用这种 `@payload.json` 方式，避免 JSON 引号被命令行转义过程吞掉。

## 导出产物

导出调试构建：

```powershell
& $godot --path $project --export-debug 'Windows Desktop' 'builds/game-debug.exe'
```

导出发布构建：

```powershell
& $godot --path $project --export-release 'Windows Desktop' 'builds/game.exe'
```

只导出项目数据：

```powershell
& $godot --path $project --export-pack 'Windows Desktop' 'builds/game.pck'
```

导出补丁包：

```powershell
& $godot --path $project --export-patch 'Windows Desktop' 'builds/patch.pck' --patches 'builds/base.pck'
```

执行导出前，确认导出预设存在于 `export_presets.cfg` 中，并且目标目录已经提前创建。

## 优先收集诊断信息

把详细输出写入文件：

```powershell
& $godot -v --path $project --log-file 'logs/godot.log' --quit-after 120
```

限制帧率并打印 FPS：

```powershell
& $godot --path $project --print-fps --max-fps 60
```

启用远程调试：

```powershell
& $godot --path $project --remote-debug 'tcp://127.0.0.1:6007'
```

切到无头模式，绕过显示和音频依赖：

```powershell
& $godot --headless --path $project --quit-after 60
```

在 Windows 上尝试不同渲染驱动：

```powershell
& $godot --path $project --rendering-driver opengl3 --quit-after 60
```

## 常用参数族

选择后续参数时，可以先按下面这些分组思考：

| 目标 | 参数 |
| --- | --- |
| 控制验证运行范围 | `--quit`, `--quit-after <n>`, `--check-only` |
| 无窗口运行 | `--headless`, `--display-driver headless`, `--audio-driver Dummy` |
| 收集日志 | `-v`, `--verbose`, `--log-file <path>`, `--no-header` |
| 指定场景 | `--scene <path-or-uid>` |
| 运行时调试 | `-d`, `--remote-debug`, `--profiling`, `--print-fps` |
| 渲染调试 | `--gpu-validation`, `--gpu-profile`, `--rendering-driver <driver>` |
| 导入或导出 | `--import`, `--build-solutions`, `--export-*`, `--patches` |
