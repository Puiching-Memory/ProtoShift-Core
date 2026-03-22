# 统一操作语义参考

ProtoShift 对外只暴露统一的操作语义，不直接让用户操作零散的引擎命令。

## 语义列表

### 项目管理

| 语义 | 说明 | Godot 实现 | Unreal 实现 |
|------|------|-----------|-------------|
| OpenProject | 打开项目 | 验证 project.godot | 验证 .uproject |
| RunProject | 运行项目 | `godot --path` | PIE_Play (Remote Control) |
| StopProject | 停止运行 | Kill process | PIE_Stop (Remote Control) |
| ReadLog | 读取日志 | 内部日志缓冲 | 内部日志缓冲 |

### 场景操作

| 语义 | 说明 | Godot 实现 | Unreal 实现 |
|------|------|-----------|-------------|
| GetSceneTree | 获取场景结构 | 读取 .tscn | Remote Control 查询 |
| CreateObject | 创建对象 | MCP create_node | SpawnActorFromClass |
| ModifyProperty | 修改属性 | MCP set_property | Remote Control property |

### 脚本与编译

| 语义 | 说明 | Godot 实现 | Unreal 实现 |
|------|------|-----------|-------------|
| CreateScript | 创建脚本 | 写文件到项目 | 写文件到项目 |
| ReadScript | 读取脚本 | 读文件 | 读文件 |
| CompileOrReload | 编译/热重载 | `--build-solutions` | CompileBlueprints |

### 资源与测试

| 语义 | 说明 | Godot 实现 | Unreal 实现 |
|------|------|-----------|-------------|
| QueryAssets | 查询资源 | 目录扫描 | EditorAssetLibrary |
| RunTests | 运行测试 | `--run-tests` | Automation RunTests |

### 迁移

| 语义 | 说明 | Godot 实现 | Unreal 实现 |
|------|------|-----------|-------------|
| ExportMigrationModel | 导出迁移模型 | 序列化项目结构 | 不适用 |
| ImportMigrationModel | 导入迁移模型 | 不适用 | 解析并生成结构 |

## 接口定义

参见 [src/EngineBridge/IEngineBridge.cs](../src/EngineBridge/IEngineBridge.cs)
