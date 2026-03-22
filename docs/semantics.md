# 统一操作语义参考

ProtoShift 对外只暴露统一语义，不要求用户理解底层 Godot、UnrealSharp 或 unreal-mcp 的具体命令。

新的语义分为两组：

1. 编辑器与项目语义
2. 宿主 Runtime 语义

## 1. 编辑器与项目语义

### 项目管理

| 语义 | 说明 | Godot 实现 | Unreal 实现 |
| --- | --- | --- | --- |
| OpenProject | 打开项目 | 验证 `project.godot` | 验证 `.uproject` |
| RunProject | 运行项目 | 启动 Godot 项目 | 启动 PIE / 编辑器运行 |
| StopProject | 停止运行 | 停止 Godot 进程 | 停止 PIE |
| ReadLog | 读取日志 | Godot 日志缓冲 | Unreal 输出日志 |

### 结构与资源

| 语义 | 说明 | Godot 实现 | Unreal 实现 |
| --- | --- | --- | --- |
| GetSceneTree | 获取宿主场景结构 | 读取 `.tscn` / MCP 查询 | 读取 Level / Actor 结构 |
| CreateObject | 创建宿主对象 | 创建节点 | 创建 Actor / Component |
| ModifyProperty | 修改宿主属性 | 设置节点属性 | 设置 Actor / Component 属性 |
| QueryAssets | 查询资源 | 目录扫描 / Godot 资源查询 | EditorAssetLibrary / MCP |

### 脚本、工程与导出

| 语义 | 说明 | Godot 实现 | Unreal 实现 |
| --- | --- | --- | --- |
| CreateScript | 创建宿主脚本 | 写入 GodotHost 或项目文件 | 写入 UnrealSharp / Blueprint 辅助文件 |
| ReadScript | 读取脚本 | 读取项目文件 | 读取项目文件 |
| CompileOrReload | 编译或热重载 | `dotnet build` / `--build-solutions` | UnrealSharp 构建 / Blueprint 编译 |
| ExportMigrationArtifacts | 导出迁移产物 | 输出 Manifest / Spec / UiSchema | 不适用 |
| ImportMigrationArtifacts | 导入迁移产物 | 不适用 | 解析并生成 Unreal 承接结构 |

## 2. 宿主 Runtime 语义

这部分是新架构的关键。ProtoShift 不是仅控制编辑器，还要让两个引擎加载同一份 `GamePackage`。

### 运行时驱动

| 语义 | 说明 | Godot Host | Unreal Host |
| --- | --- | --- | --- |
| BootGamePackage | 启动项目级 `GamePackage` | 加载纯 .NET 游戏包并注入服务 | UnrealSharp 加载同一游戏包 |
| TickRuntime | 推进一帧游戏逻辑 | `_Process` / `_PhysicsProcess` 转发 | `Tick` / 定时器转发 |
| DispatchInput | 提交输入命令 | 输入事件 → 领域命令 | 输入映射 → 领域命令 |
| ReadPhysicsSnapshot | 读取物理反馈 | Godot 物理采样 | Unreal 物理采样 |
| ApplyGameplayOutput | 应用游戏包输出 | 同步节点、动画、音效、UI | 同步 Actor、组件、UMG |
| CaptureGameplaySnapshot | 捕获玩法快照 | 运行时状态序列化 | 运行时状态序列化 |
| CompareGameplaySnapshot | 比较迁移一致性 | 与基准快照比对 | 与基准快照比对 |

### UI 语义

| 语义 | 说明 | Godot Host | Unreal Host |
| --- | --- | --- | --- |
| LoadUiSchema | 载入 UI Schema | Control 映射 | UMG 映射 |
| BindViewModel | 绑定 ViewModel | 数据绑定到 Control | 数据绑定到 Widget |
| DispatchUiEvent | 回传 UI 事件 | 点击 / 悬停 / 输入 | 点击 / Hover / 输入 |

## 迁移产物

迁移阶段不再只导出一个“迁移模型 JSON”，而是导出一组明确分层的产物：

1. `Scene Manifest`
2. `Gameplay Spec`
3. `UiSchema`
4. `ViewModel Contract`
5. `host-only backlog`

其中：

- `Scene Manifest` 描述结构、资源、宿主对象映射
- `Gameplay Spec` 描述命令、事件、状态与规则边界
- `host-only backlog` 明确记录仍依赖 Godot 的部分，不允许隐式丢失

## 接口方向

现有 [standard-template/Core/EngineBridge/IEngineBridge.cs](../standard-template/Core/EngineBridge/IEngineBridge.cs) 仍可继续承载编辑器操作语义。

后续所有运行时设计应围绕以下宿主契约组织：

- `IHostClock`
- `IHostInput`
- `IHostPhysics`
- `IHostScene`
- `IHostUi`
- `IGameRuntimeHost`

即使具体接口文件尚未全部实现，Agent 的生成与迁移行为也必须遵守这一边界。
