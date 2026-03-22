# ProtoShift-Core 简化执行方案

## 1. 最终产品定义

ProtoShift-Core 只做一件事：

- 把 opencode 变成一个面向游戏开发的定制化 Agent 工作台
- 用户全程通过自然语言和同一个 shell 交互
- Agent 先在 Godot 中完成原型开发
- 原型稳定后，再在同一项目中迁移到 Unreal 5 继续开发

这不是重写一套新 Agent，也不是同时维护两套独立工作流，而是在 opencode 之上加一层面向 Godot/Unreal 的产品化能力。

## 2. 固定技术路线

主线固定如下：

- `opencode` 以 git submodule 方式集成，作为主宿主和主交互入口
- ProtoShift 只做二次开发：instructions、commands、hooks、skills、engine bridge
- Godot 负责原型生成和快速迭代
- Unreal 5 负责后续承接和正式开发
- 共享逻辑优先写在 C# Shared Core 中
- Godot 直接使用原生 C#
- Unreal 通过 UnrealSharp 接入 Shared Core
- 引擎控制层同时兼容 MCP 和 CLI，两者都作为后端实现
- UI 不追求两个引擎控件完全一致，而是定义一层通用 UI Schema，再分别映射到 Godot 和 Unreal

## 3. ACT 原则

ProtoShift-Core 采用 ACT 原则：

- A - Adapter First：先定义统一语义，再接入 Godot MCP/CLI 和 Unreal MCP/CLI
- C - Code Reuse：玩法规则、状态机、任务系统、配置、存档和 UI 描述尽量共享
- T - Tool Driven：所有核心流程优先通过 opencode + skills + MCP/CLI 驱动，不依赖人工点编辑器完成主工作流

## 4. 最小架构

为了避免过度设计，第一阶段只保留四层。

### 4.1 opencode Host

负责：

- 自然语言 shell
- 会话与计划推进
- 通用工具调用
- skill 装载和调度

不负责：

- 游戏语义定义
- Godot / Unreal 适配逻辑
- 共享规则设计

### 4.2 ProtoShift Extension

负责：

- ProtoShift 专用 instructions、commands、hooks
- 面向游戏开发的 skills
- 工作流约束和上下文注入
- 把用户请求翻译成统一的引擎操作语义

### 4.3 Shared Core

负责：

- 共享 C# 规则层
- 状态机、任务、对话、配置、存档
- 原型描述模型
- 通用 UI Schema

不负责：

- Godot Node 生命周期
- Unreal Actor / Component 生命周期
- 引擎资源导入细节
- 引擎特有 UI 控件实现

### 4.4 Engine Bridge

负责：

- 统一 Godot 和 Unreal 的操作语义
- 对下适配 MCP、CLI、Python、插件桥接等后端
- 把日志、测试结果和运行状态回传给 opencode

## 5. 统一操作语义

对外只暴露 ProtoShift 语义，不直接让用户操作零散引擎命令。

第一批统一语义建议固定为：

- 打开项目
- 运行项目
- 停止运行
- 读取日志
- 获取场景 / 关卡结构
- 创建对象
- 修改属性
- 创建脚本
- 读取脚本
- 编译或热重载
- 查询资源
- 运行测试
- 导出迁移模型
- 从迁移模型生成 Unreal 侧结构

## 6. 通用 UI 系统

UI 主线不做“跨引擎控件直接复用”，而是做“共享描述 + 双端映射”。

第一阶段只支持高迁移价值的 UI 能力：

- Layout：纵向、横向、网格、滚动、叠层、间距、边距
- Components：文本、图片、按钮、面板、列表、进度条、输入框
- Theme：颜色、字号、字体、间距、圆角、描边、状态样式
- Binding：文本绑定、数值绑定、可见性绑定、列表绑定
- Events：点击、悬停、选中、禁用、焦点

实现方式：

- Shared Core 保存 UI Schema 和 ViewModel
- Godot Adapter 将其映射到 Control 体系
- Unreal Adapter 将其映射到 UMG 或 Slate
- 引擎专有复杂控件放在各自适配层扩展，不污染共享 Schema

## 7. 最终使用流程

目标使用流程固定如下：

1. 用户启动定制化的 opencode。
2. 用户用自然语言描述要做的游戏原型。
3. Agent 调用 ProtoShift skills，并通过 Godot MCP/CLI 创建和修改原型。
4. 共享玩法规则、数据模型和 UI Schema 持续沉淀到 Shared Core。
5. 用户确认原型方向稳定后，Agent 导出迁移模型。
6. Agent 调用 Unreal MCP/CLI 和 UnrealSharp，在同一项目中生成 Unreal 侧承接结构。
7. 用户继续通过同一个自然语言入口推进后续开发，而不是切换产品或重写玩法逻辑。

## 8. 第一阶段必须交付的东西

第一阶段不追求大而全，只交付真正影响主线闭环的部分：

1. opencode submodule 集成完成
2. ProtoShift 自定义 instructions / commands / hooks 骨架
3. Godot Bridge：MCP + CLI 最小可用能力
4. Unreal Bridge：MCP + CLI 最小可用能力
5. Shared Core：最小 C# 共享规则工程
6. UnrealSharp 接入 Shared Core 的验证样例
7. 通用 UI Schema 的最小版本
8. 从 Godot 原型到 Unreal 承接的最小迁移样例

## 9. 建议目录结构

建议按最小落地结构组织：

- `opencode/`
- `src/OpenCodeExtension`
- `src/SharedCore`
- `src/EngineBridge`
- `src/GodotBridge`
- `src/UnrealBridge`
- `src/UiSchema`
- `docs/`
- `samples/`

## 10. 不做的事情

以下内容不进入第一阶段：

- 自建一整套通用 Agent runtime 替代 opencode
- 追求全部引擎层代码跨引擎通用
- 同时维护多套 Unreal C# 方案
- 一开始就覆盖复杂特效控件和引擎专有 UI 时间线
- 把主要工作流建立在截图理解和人工点编辑器上

## 11. 当前结论

ProtoShift-Core 的正确方向不是继续扩展抽象层，而是先完成这个最小闭环：

- 基于 opencode 做定制化宿主
- 接通 Godot 和 Unreal 的 MCP / CLI 工具
- 用 Shared Core 复用尽可能多的 C# 逻辑
- 用通用 UI Schema 保持 UI 可迁移
- 让用户始终通过同一个自然语言入口完成从 Godot 原型到 Unreal 承接的全流程
- src/AgentProtocol
- src/Tooling
- src/OpenCodeExtension
- src/Skills
- docs
- examples
- samples

### 3.3 对外执行入口

系统对外只暴露 ProtoShift 操作语义，不直接暴露零散的引擎命令。

这意味着：

- 用户面对的是 OpenCode Shell 中的 ProtoShift 语义能力，而不是零散后端命令
- Godot MCP、Unreal MCP、Godot CLI、Unreal Python、Remote Control 都作为内部后端存在
- OpenCode 是主壳层，但上层工作流由 ProtoShift 语义和 Skills 驱动

### 3.4 执行层最终决策

执行层固定采用“OpenCode 主壳层 + ProtoShift Skills 与扩展层”的方案。

第一阶段主线如下：

- 用户通过自然语言在终端中与 ProtoShift 交互
- 用户使用层直接运行 OpenCode 风格的终端 Agent，并通过 ProtoShift 扩展获得游戏引擎工作流能力
- OpenCode 作为主壳层、主会话系统和通用 Agent runtime
- ProtoShift 基于 OpenCode 做二次开发，而不是完全自建一个新的通用后端
- 交互模型遵循 one bash 理念：用户进入同一个 shell 后完成提问、规划、执行、调试、测试与回归
- OpenCode Shell 承载会话、计划和通用工具调用，ProtoShift 扩展在其中注入引擎语义、测试编排和领域约束
- ProtoShift 在内部挂接 Godot MCP、Unreal MCP、CLI、Python、Remote Control 等后端，并统一映射到 ProtoShift 语义动作

OpenCode 主壳层固定负责：

- one bash 单入口 shell
- 自然语言会话与命令控制面
- 通用 Skills 装载、发现、路由和版本管理
- 会话状态、计划推进和通用代码代理体验

ProtoShift 扩展层固定负责：

- ProtoShift 专用 instructions、commands、hooks 和工作流约束
- Shared Core、Agent Bridge、Godot Adapter、Unreal Adapter 的上下文注入
- 测试、回归、日志、快照和回放的统一编排
- 将用户请求和工具执行统一映射回 ProtoShift 语义结果

ProtoShift Skills 固定负责：

- 将领域能力按高层语义拆分为可组合的能力单元
- 为 Godot、Unreal、Shared Core、测试、迁移、空间语义解码提供独立技能边界
- 让 OpenCode Shell 基于上下文选择、串联和约束不同技能
- 保持技能层可扩展，而不污染共享核心与引擎适配层

该决策的目标不是抽象参考 OpenCode，而是直接站在 OpenCode 之上做产品化扩展，把研发资源集中在 ProtoShift 自身真正不可替代的能力上：

- 跨引擎共享规则层
- ProtoShift 统一语义层
- ProtoShift Skills 与 OpenCode 扩展层
- Godot 与 Unreal 适配层
- 自动化验证与修复闭环
- 空间语义解码与原型迁移能力

如果未来需要替换 OpenCode、增加桌面前端或引入远程客户端，这些调整只能发生在主壳层之外或作为后续迁移工程，不改变 ProtoShift 语义层、Skills 驱动和 one bash 工作流优先的总原则。

---

## 四、最终能力范围

### 4.1 Shared Core

Shared Core 最终负责：

- 统一玩法规则、状态机、任务系统、对话系统、行为树数据结构和 AI 决策逻辑
- 统一配置解析、存档模型和原型 DSL
- 提供可测试、可回放、可断言的规则表达
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

