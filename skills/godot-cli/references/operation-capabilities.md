# 引擎内操作能力参考

当 Godot CLI 任务已经超出“启动、导入、导出、检查”这类简单命令时，可以先把它拆成一组最小单功能脚本，再按能力归类。

这些拆分主要是为了复用和减少试错，不是唯一的目录结构或实现方式。一次性任务可以直接写临时脚本；反复出现的任务再考虑沉淀成参考能力。

## 单功能脚本核心能力

常见做法是把引擎内修改统一成稳定调用形态。

大多数项目内修改脚本：

```powershell
& $godot --headless --path $project --script 'tools/<tool-script.gd>' '<json-payload>'
```

依赖真实渲染结果的脚本：

```powershell
& $godot --path $project --script 'tools/<tool-script.gd>' '@payload.json'
```

这套模式通常会提供这些能力：

- 每个脚本只做一件事，不再额外解析操作名。
- 把项目相对路径规范化为 Godot 可读取的资源路径。
- 在失败时直接输出错误并返回非零退出。
- 把公共逻辑抽成共享 helper，例如路径转换、场景加载、节点定位、目录创建、场景保存、资源递归扫描。
- 让单功能脚本本身只保留最小业务逻辑。

对依赖真实渲染结果的工具，例如场景截图，不要强行塞进 `--headless`。这类脚本应改用普通 `--script`。

## 代码拆分建议

常见参考拆分是下面两层：

- 共享 helper：`tool_common.gd`
- 单功能脚本：`tool_create_scene.gd`、`tool_add_node.gd`、`tool_load_sprite.gd`、`tool_export_mesh_library.gd`、`tool_save_scene.gd`、`tool_get_uid.gd`、`tool_resave_resources.gd`、`tool_capture_scene.gd`
- 特殊初始化脚本：`tool_init_project.gd`

如果当前任务更适合更高层封装、一次性脚本或批处理入口，也可以直接调整这套拆分。

## 能力分组

### 1. 场景骨架能力

适用脚本：`tool_create_scene.gd`、`tool_save_scene.gd`

这一组负责从零生成场景，或把当前场景另存为变体。

- `tool_create_scene.gd`
  - 输入：`scene_path`，可选 `root_node_type`
  - 结果：创建根节点、打包成 `PackedScene`、确保目录存在并保存
  - 关键 API：`ClassDB.instantiate()`、`PackedScene.pack()`、`ResourceSaver.save()`、`DirAccess.make_dir_recursive()`
- `tool_save_scene.gd`
  - 输入：`scene_path`，可选 `new_path`
  - 结果：加载现有场景、重新打包、覆盖保存或另存为新路径
  - 关键 API：`load()`、`PackedScene.pack()`、`ResourceSaver.save()`

### 2. 场景增量编辑能力

适用脚本：`tool_add_node.gd`、`tool_load_sprite.gd`

这一组负责在已有场景上做最小修改，而不是重建整个场景。

- `tool_add_node.gd`
  - 输入：`scene_path`、`node_type`、`node_name`，可选 `parent_node_path`、`properties`
  - 结果：实例化节点、挂到父节点、设置 owner、重存场景
  - 关键 API：`get_node()`、`add_child()`、`set()`、`PackedScene.pack()`
- `tool_load_sprite.gd`
  - 输入：`scene_path`、`node_path`、`texture_path`
  - 结果：定位 `Sprite2D`、`Sprite3D` 或 `TextureRect`，加载纹理并写回场景
  - 关键 API：`load()`、类型判断、节点路径归一化

### 3. 资源导出能力

适用脚本：`tool_export_mesh_library.gd`

这一组把场景中的 3D 资源整理成适合复用的导出物。

- 输入：`scene_path`、`output_path`，可选 `mesh_item_names`
- 结果：遍历场景子节点，提取 `MeshInstance3D`、碰撞体和预览信息，写入 `MeshLibrary`
- 关键 API：`MeshLibrary.new()`、`set_item_mesh()`、`set_item_shapes()`、`set_item_preview()`

### 4. 标识与资源维护能力

适用脚本：`tool_get_uid.gd`、`tool_resave_resources.gd`

这一组偏向项目维护，而不是直接改玩法内容。

- `tool_get_uid.gd`
  - 输入：`file_path`
  - 结果：读取目标资源的 `.uid` 文件并输出结构化信息
  - 关键 API：`FileAccess.open()`、`get_as_text()`、`JSON.stringify()`
- `tool_resave_resources.gd`
  - 输入：可选 `project_path`
  - 结果：递归重存 `.tscn`、`.gd`、`.shader`、`.gdshader`，补齐缺失 UID
  - 关键 API：`DirAccess` 递归扫描、`ResourceSaver.save()`、`load()`

### 5. 类型解析与节点定位能力

这部分不是独立脚本，但几乎是共享 helper 的基础设施。

- 根据类名或脚本路径实例化节点
- 把 `root/...` 风格节点路径转成实际查询路径
- 在属性写入时识别可加载资源并替换为资源对象
- 统一场景保存逻辑，避免每个脚本重复打包与落盘代码
- 统一 payload 解析逻辑，支持直接 JSON 和 `@payload.json`

### 6. 项目初始化能力

适用脚本：`tool_init_project.gd`

这一组不依赖现有项目上下文，负责从零生成一个可被 Godot 识别的项目目录。

- 输入：`target_dir`，可选 `project_name`、`create_main_scene`、`main_scene_path`、`root_node_type`、`folders`
- 结果：创建项目目录、常用子目录、可选主场景，并写出最小 `project.godot`
- 关键点：这个脚本不应依赖 `res://` helper，也不应要求先有 `--path`

### 7. 渲染截图能力

适用脚本：`tool_capture_scene.gd`

这一组负责把某个场景按真实渲染结果输出成图片，而不是只做资源层修改。

- 输入：`scene_path`、`output_path`，可选 `resolution`、`wait_frames`、`camera_node_path`
- 结果：实例化场景、可选切换相机、等待若干帧后抓取视口，并写出 PNG 文件
- 关键点：不要使用 `--headless`，否则拿不到可信的渲染结果
- 关键 API：`Viewport.get_texture()`、`ViewportTexture.get_image()`、`Image.save_png()`、`Camera2D/3D.make_current()`

## 常见组合路径

最常见的组合不是单个脚本，而是流水线：

1. `tool_create_scene.gd` 创建骨架
2. `tool_add_node.gd` 添加节点树
3. `tool_load_sprite.gd` 或其他资源写入脚本补内容
4. `tool_save_scene.gd` 生成变体或最终版本

另一个常见路径是维护流水线：

1. `tool_get_uid.gd` 检查单个资源状态
2. `tool_resave_resources.gd` 批量修复项目中的 UID 和资源引用

从零开始的另一条路径是：

1. `tool_init_project.gd` 初始化项目目录
2. `tool_create_scene.gd` 或直接使用初始化时生成的主场景
3. `tool_add_node.gd`、`tool_load_sprite.gd` 等脚本继续扩展内容

另一条常见路径是内容验收流水线：

1. `tool_create_scene.gd` 或 `tool_add_node.gd` 生成要检查的场景
2. `tool_capture_scene.gd` 输出截图
3. 基于截图继续做视觉回归或人工检查

## 如何使用这些参考资源

- 想知道应该做成哪类能力：先读这份文件。
- 想知道每个操作该收什么参数：再读 [operation-contracts.md](operation-contracts.md)。
- 想直接拿最小代码：复制 [../assets/tool_common.gd](../assets/tool_common.gd) 和目标 `tool_*.gd`。
- 想初始化一个新项目：直接使用 [../assets/tool_init_project.gd](../assets/tool_init_project.gd)。
- 想输出场景截图：直接使用 [../assets/tool_capture_scene.gd](../assets/tool_capture_scene.gd)。
