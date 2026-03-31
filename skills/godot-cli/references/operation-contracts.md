# 引擎内操作参数契约

这里整理的是单功能脚本的最小操作契约，用来提供稳定默认形态，而不是穷尽所有可能字段或流程。

如果当前任务明显需要更多上下文、更高层封装或额外字段，可以在这些契约上继续扩展。

## 通用调用形态

大多数项目内脚本：

```powershell
& $godot --headless --path $project --script 'tools/<tool-script.gd>' '<json-payload>'
```

依赖真实渲染结果的脚本：

```powershell
& $godot --path $project --script 'tools/<render-tool.gd>' '@payload.json'
```

约定：

- 复制单功能脚本时，把 `tool_common.gd` 一起放进同一套 `tools/` 目录。
- `scene_path`、`texture_path`、`output_path`、`file_path` 优先写项目相对路径。
- `parent_node_path` 和 `node_path` 可以写成 `root/Player/Sprite2D` 这种形式。
- 未提供的可选字段由脚本内给默认值，不必把所有字段都硬塞进 payload。
- 共享 helper 脚本支持直接传 JSON，也支持传 `@payload.json`。在 Windows 自动化里，后者更稳。
- `tool_init_project.gd` 是例外：它面向“尚未存在的项目”，用的是目标项目目录 `target_dir`，而不是 `--path` 下的项目相对路径。
- `tool_capture_scene.gd` 也是例外：它依赖真实渲染结果，调用时不要带 `--headless`。

## init_project

脚本：`tool_init_project.gd`

最小 payload：

```json
{"target_dir":"<target-directory>"}
```

常用扩展：

```json
{"target_dir":"<target-directory>","project_name":"My Game","create_main_scene":true,"main_scene_path":"scenes/Main.tscn","root_node_type":"Node2D","folders":["scenes","scripts","assets"]}
```

CLI 模板：

```powershell
$target = '<target-directory>'
$payloadFile = Join-Path $env:TEMP 'godot-init-payload.json'
@{
  target_dir = $target
  project_name = 'My Game'
  create_main_scene = $true
} | ConvertTo-Json -Compress | Set-Content -LiteralPath $payloadFile -Encoding UTF8

& $godot --headless --script '.\tool_init_project.gd' "@$payloadFile"
```

说明：

- 这里不要带 `--path`。
- `target_dir` 指向目标项目目录本身。
- 这是唯一一个默认不依赖 `tool_common.gd` 的脚本，因为它运行时项目还不存在。
- 这个脚本既支持直接传入 JSON 字符串，也支持传入 `@payload.json`。在 Windows 自动化里，后者更稳。

## create_scene

脚本：`tools/tool_create_scene.gd`

最小 payload：

```json
{"scene_path":"scenes/Test.tscn"}
```

常用扩展：

```json
{"scene_path":"scenes/Test.tscn","root_node_type":"Node3D"}
```

CLI 模板：

```powershell
& $godot --headless --path $project --script 'tools/tool_create_scene.gd' '{"scene_path":"scenes/Test.tscn","root_node_type":"Node2D"}'
```

## add_node

脚本：`tools/tool_add_node.gd`

最小 payload：

```json
{"scene_path":"scenes/Test.tscn","node_type":"Sprite2D","node_name":"Player"}
```

常用扩展：

```json
{"scene_path":"scenes/Test.tscn","parent_node_path":"root/Actors","node_type":"CollisionShape2D","node_name":"Hitbox","properties":{"disabled":false}}
```

CLI 模板：

```powershell
& $godot --headless --path $project --script 'tools/tool_add_node.gd' '{"scene_path":"scenes/Test.tscn","parent_node_path":"root","node_type":"Sprite2D","node_name":"Player"}'
```

## load_sprite

脚本：`tools/tool_load_sprite.gd`

最小 payload：

```json
{"scene_path":"scenes/Test.tscn","node_path":"root/Player","texture_path":"textures/player.png"}
```

CLI 模板：

```powershell
& $godot --headless --path $project --script 'tools/tool_load_sprite.gd' '{"scene_path":"scenes/Test.tscn","node_path":"root/Player","texture_path":"textures/player.png"}'
```

## export_mesh_library

脚本：`tools/tool_export_mesh_library.gd`

最小 payload：

```json
{"scene_path":"scenes/Tileset.tscn","output_path":"resources/tileset_mesh_library.tres"}
```

常用扩展：

```json
{"scene_path":"scenes/Tileset.tscn","output_path":"resources/tileset_mesh_library.tres","mesh_item_names":["Grass","Wall","Ramp"]}
```

CLI 模板：

```powershell
& $godot --headless --path $project --script 'tools/tool_export_mesh_library.gd' '{"scene_path":"scenes/Tileset.tscn","output_path":"resources/tileset_mesh_library.tres"}'
```

## save_scene

脚本：`tools/tool_save_scene.gd`

最小 payload：

```json
{"scene_path":"scenes/Test.tscn"}
```

另存为变体：

```json
{"scene_path":"scenes/Test.tscn","new_path":"scenes/TestVariant.tscn"}
```

CLI 模板：

```powershell
& $godot --headless --path $project --script 'tools/tool_save_scene.gd' '{"scene_path":"scenes/Test.tscn","new_path":"scenes/TestVariant.tscn"}'
```

## get_uid

脚本：`tools/tool_get_uid.gd`

最小 payload：

```json
{"file_path":"scripts/player.gd"}
```

CLI 模板：

```powershell
& $godot --headless --path $project --script 'tools/tool_get_uid.gd' '{"file_path":"scripts/player.gd"}'
```

## resave_resources

脚本：`tools/tool_resave_resources.gd`

最小 payload：

```json
{}
```

限制到子目录：

```json
{"project_path":"addons/my_tool"}
```

CLI 模板：

```powershell
& $godot --headless --path $project --script 'tools/tool_resave_resources.gd' '{}'
```

## capture_scene

脚本：`tools/tool_capture_scene.gd`

最小 payload：

```json
{"scene_path":"scenes/Main.tscn","output_path":"artifacts/main-scene.png"}
```

常用扩展：

```json
{"scene_path":"scenes/Main.tscn","output_path":"artifacts/main-scene.png","resolution":[1280,720],"wait_frames":4,"camera_node_path":"root/Camera3D"}
```

CLI 模板：

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

说明：

- 这里不要带 `--headless`。
- `output_path` 当前应以 `.png` 结尾。
- `resolution` 可以写成 `"1280x720"`、`[1280, 720]`，或 `{ "width": 1280, "height": 720 }`。
- 对 3D 场景，通常需要场景内已有可用相机，或通过 `camera_node_path` 显式切换。

## 推荐的参数收敛方式

- 只把脚本真正需要的字段放进 payload。
- 场景、贴图、脚本路径都优先使用项目相对路径。
- 需要写资源属性时，把纯量值和资源路径区分开；资源路径由共享 helper 统一加载。
- 需要支持更多操作时，先扩展单功能脚本；只有共享入口很有价值时，再叠加 dispatcher。
