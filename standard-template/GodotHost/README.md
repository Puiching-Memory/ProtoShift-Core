# GodotHost

该目录是 ProtoShiftGame 的 Godot 宿主壳层。

职责：

- 采集 Godot 输入
- 驱动场景与 UI
- 读取物理与宿主反馈
- 调用 `GamePackage` 运行时

限制：

- 不保存玩法真相
- 不定义项目级规则
- 任何宿主专属逻辑都应同步记录到 `Migration/host-only-backlog.json`