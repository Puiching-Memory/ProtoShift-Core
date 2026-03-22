# UnrealProject

该目录提供可直接承接 `GamePackage` 的 Unreal 宿主骨架。

已提供：

- `ProtoShiftGame.uproject`：Unreal 5 项目入口
- `Managed/ProtoShiftGame.UnrealHost.csproj`：托管宿主工程
- `Managed/TemplateUnrealHostCoordinator.cs`：最小宿主编排器
- `Config/input-actions.json`：建议输入动作映射
- `Config/widget-bindings.json`：建议 UMG 绑定关系

刻意不提供：

- `Binaries/`、`DerivedDataCache/`、`Intermediate/`、`Saved/` 等本机生成目录
- Unreal 或 UnrealSharp 首次同步后生成的 `.sln`、`Script/` 与调试启动配置

打开方式：

- 用 Unreal 5 打开 `ProtoShiftGame.uproject`
- 再由 UnrealSharp 托管层加载 `Managed/` 与项目级 `GamePackage`
- 若引擎或插件需要生成额外脚本/解决方案，应在本机工作区生成，而不是回写模板

接入 UnrealSharp 时，优先保留以下边界：

- `GamePackage/` 继续保存玩法真相
- Unreal 宿主只负责输入、场景和 UMG 同步
- 如需引擎专属能力，记录回 `Migration/host-only-backlog.json`