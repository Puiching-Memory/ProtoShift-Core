# ProtoShift-Core 落地蓝图

本文件不再记录开放式待办，而用于固定 ProtoShift-Core 的最终落地边界。

## 1. 产品定义

ProtoShift-Core 只做一件事：

- 让 Agent 在 Godot 中快速做原型
- 并在原型稳定后，把同一份项目级 C# 游戏包承接到 Unreal 5

重点不是“把 Godot 脚本迁移到 Unreal”，而是“从第一天起就不把玩法写成 Godot 专属代码”。

## 2. 固定技术路线

- opencode 作为主宿主与会话层
- ProtoShift 负责 instructions、commands、skills、MCP 与工作流约束
- Godot 负责原型验证与手感调试
- Unreal 5 + UnrealSharp 负责正式承接
- unreal-mcp 负责 Unreal 编辑器内结构生成和自动化操作
- `standard-template/Core/SharedCore/` 只保留平台级能力
- 每个游戏在 `workspace/<slug>/GamePackage/` 中维护自己的可移植玩法包

## 3. CAT 原则

- C - Code Reuse：复用项目级 `GamePackage`、快照测试、ViewModel 和 UiSchema
- A - Adapter Design：Godot Host 与 Unreal Host 只在框架层实现一次
- T - Token-friendly：Agent 优先生成 C# 游戏包、Manifest、Schema 和绑定配置

## 4. 固定分层

### 4.1 平台层

路径：

- `standard-template/Core/SharedCore/`
- `standard-template/Core/EngineBridge/`
- `standard-template/Core/GodotBridge/`
- `standard-template/Core/UnrealBridge/`
- `standard-template/Core/UiSchema/`

职责：

- 提供通用类型、桥接语义与宿主协议
- 不承载任何具体游戏规则

### 4.2 项目层

路径：

- `workspace/<slug>/GamePackage/`
- `workspace/<slug>/GamePackage.Tests/`
- `workspace/<slug>/Migration/`

职责：

- 保存该项目唯一玩法真相
- 保存测试、快照、Manifest、Spec、UiSchema 和 backlog

### 4.3 宿主层

路径：

- `workspace/<slug>/GodotHost/`
- `workspace/<slug>/UnrealProject/`

职责：

- 把宿主输入、物理、UI 和场景同步接到同一份 `GamePackage`

## 5. 原型与迁移闭环

### Godot 原型阶段

1. 初始化 `workspace/<slug>/`
2. 创建 `GamePackage/`、`Migration/`、Godot 宿主壳层
3. 玩法逻辑优先写入 `GamePackage/`
4. Godot 只负责 Host 接线与运行验证
5. 通过 Godot MCP 完成真实运行闭环

### 稳定后导出

1. 导出 `Scene Manifest`
2. 导出 `Gameplay Spec`
3. 导出 `UiSchema/ViewModel`
4. 产出 `host-only backlog`

### Unreal 承接阶段

1. unreal-mcp 创建 Unreal 工程骨架
2. UnrealSharp 加载同一份 `GamePackage`
3. `Scene Manifest` 映射为 Actor/Component
4. `UiSchema` 映射为 UMG
5. `host-only backlog` 单独处理，不隐式丢失

### 验证阶段

1. `GamePackage.Tests` 跑纯 .NET 测试
2. Godot 仅验证 Godot Host 行为
3. Unreal 仅验证 Unreal Host 行为
4. 使用 gameplay snapshot 对比迁移前后的一致性

## 6. 固定约束

- 不把项目规则写入 `standard-template/Core/SharedCore/`
- 不让 `GamePackage/` 引用 `Godot.*` 或 `UnrealSharp.*`
- 不把原型迁移建立在双端玩法脚本重写上
- 不把 host-only 行为混入共享玩法层
- 不把主要工作流建立在截图理解和手工点编辑器上
- 统一管理空间知识、资产元数据和设计标记语义

### 4.2 OpenCode Host

OpenCode Host 最终负责：

- 提供用户自然语言交互入口
- 管理会话、计划和通用工具调用
- 承载通用 Skills 的装载、路由和执行上下文
- 为 ProtoShift 扩展提供稳定的主壳层与运行时环境

### 4.3 ProtoShift Extension

ProtoShift Extension 最终负责：

- 把 OpenCode 的通用 Agent 能力映射到 ProtoShift 游戏原型工作流
- 注入 ProtoShift 语义约束、工作流规则和领域上下文
- 管理 ProtoShift Skills、命令、hooks、日志和验证编排
- 统一连接 Shared Core、Agent Bridge 和各引擎适配器

### 4.4 Godot

Godot 最终负责：

- 由 Agent 创建和修改场景、节点、脚本和资源引用
- 启动、停止和调试原型运行
- 将共享核心逻辑映射为可运行的 Godot 场景与脚本结构
- 导出或读取包围盒、碰撞体、Marker、Anchor 和区域语义
- 运行集成测试和自动游玩测试，并输出关键状态和轨迹

### 4.5 Unreal

Unreal 最终负责：

- 通过 UnrealSharp 接入共享核心逻辑
- 将共享对象模型映射为 Actor、Component 和 Blueprint 可访问对象
- 执行关卡对象操作、属性修改、资源查询和编辑器自动化
- 读取 Socket、Volume 和关卡标记的空间元数据
- 运行功能验证、长序列集成测试和自动游玩测试

### 4.6 Agent Bridge 与平台能力

平台最终负责：

- 对外暴露统一 ProtoShift 操作语义
- 对内适配多种后端实现
- 保持 OpenCode Host、扩展层、共享层、适配层、桥接层边界清晰
- 以分层测试作为自动化迭代与缺陷修复的终止条件
- 运行 Autonomous Bug Fix Loop，自动执行验证、定位失败并触发回归修复
- 通过 Knowledge、Asset Metadata、Design Metadata 完成空间语义解码
- 支持 Godot 原型到 Unreal 承接的同构工作流

### 4.7 跨引擎 UI

UI 层采用自建的 ProtoShift UI Schema，不依赖现成双端 UI 框架直接完成 Godot 到 Unreal 的无损迁移。

最终能力范围如下：

- 用统一描述格式表达布局树、组件层级、样式令牌、交互事件和数据绑定
- 在 Godot 中以原生 Control 体系或声明式 UI 插件预览和编辑 UI
- 在 Unreal 中将同一份描述映射到 UMG 或 Slate
- 共享颜色、间距、字号、字体、圆角、描边和状态样式等主题令牌
- 支持容器、文本、图片、按钮、进度条、列表、面板、标签页和简单输入控件等高可迁移组件
- 支持通过适配器扩展引擎专有组件，而不污染通用描述层

UI Schema 的通用范围固定为：

- 布局容器：纵向、横向、网格、流式、边距、滚动、叠层
- 基础组件：文本、图片、按钮、进度条、面板、列表、输入框
- 样式系统：颜色、字体、字号、边距、间距、对齐、尺寸约束、状态样式
- 交互系统：点击、悬停、选中、禁用、焦点、简单动画状态
- 绑定系统：文本绑定、数值绑定、可见性绑定、列表数据绑定

不纳入通用层的内容包括：

- 复杂特效控件
- 引擎专有动画时间线
- 深度依赖材质或渲染管线的视觉组件
- 高度定制的编辑器控件

UI 资源固定分为：

- Layout
- Theme
- Assets
- ViewModel
- Overrides

---

## 五、验证样例

### 5.1 《8球（8-Ball Pool）》

定位：验证高精度物理玩法中的规则表达、空间放置和自动回归能力。

重点覆盖：

- 台面、球袋和球组规则
- 瞄准、力度和击球方向
- 球体碰撞、反弹、滚动和停球判定
- 进袋、犯规、球权切换和胜负判定
- 基于桌面尺寸、球体碰撞体和袋口标记的精确摆放
- 规则修复后单元测试、长序列击球测试和自动游玩回归同时通过

价值：验证系统不仅能生成场景，还能稳定承载精细规则与物理玩法。

### 5.2 俯视角自动射击游戏

定位：验证高频战斗循环、批量实体和参数驱动玩法。

重点覆盖：

- 角色移动与自动攻击
- 大量敌人生成、追踪和死亡处理
- 武器升级、伤害成长、被动技能和局内选择
- 掉落物、经验值、等级提升和局内构筑
- 高频战斗日志、运行反馈和参数调优流程
- 自动修复循环基于失败波次或异常成长曲线完成回归修正

价值：验证共享逻辑能否稳定承载持续运行的高密度战斗玩法。

### 5.3 第一人称 Boss 战游戏

定位：验证复杂 Gameplay Mechanics、角色层级和长战斗序列。

重点覆盖：

- 第一人称移动、瞄准、射击和受击反馈
- 命中扫描、投射物和特殊武器类型
- 普通敌人、精英敌人、支援角色和 Boss 的多层级角色体系
- Boss 多阶段战斗结构与多种攻击模式
- 关卡事件、弱点暴露、环境交互和战斗节奏切换
- 自动游玩测试稳定覆盖复杂战斗序列，并为修复循环提供有效失败证据

价值：验证系统能否从简单原型上升到复杂战斗设计，并检验 Godot 到 Unreal 承接的上限场景。

### 5.4 总体覆盖关系

这三类样例共同证明 ProtoShift-Core 具备以下能力：

- 能生成不同品类的游戏原型，而不是单一模板
- 能让共享核心承载规则、状态和数值，而不是只生成表现层
- 能让 Godot 承担高效率原型迭代，让 Unreal 承担复杂玩法承接
- 能让分层测试驱动自动修复循环，而不是依赖人工重复回归
- 能让 Agent 在场景、逻辑、规则和编辑器自动化四个层面同时工作

---

## 六、最终摘要

ProtoShift-Core 的最终执行方案可以归纳为：

- Godot 是 Agent 驱动的原型引擎
- Shared Core 是跨引擎纯 .NET 规则层
- Unreal 是正式开发承接引擎
- UnrealSharp 是 Unreal 侧唯一 C# 运行层
- OpenCode 是第一阶段主壳层与主运行时，ProtoShift 在其上提供领域扩展
- ProtoShift Skills 与扩展层负责把通用 Agent 能力转化为游戏原型工作流
- ProtoShift 统一操作语义是对外唯一执行入口
- 分层测试是自动化迭代终止条件，Autonomous Bug Fix Loop 是缺陷收敛机制
- Knowledge、Asset Metadata、Design Metadata 共同构成空间理解输入层
- MCP、CLI、Python、Remote Control 等能力都只是可替换后端，不是系统边界

