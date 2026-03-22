---
name: test
description: "运行 ProtoShift 分层测试"
agent: protoshift
---

运行 ProtoShift 分层测试套件。

默认要求：
- 必须优先运行并维护当前项目自己的 `GamePackage.Tests`，不能只依赖模板遗留测试
- 若发现测试仍然引用模板运行时、模板命令名、模板 snapshot 或模板文档语义，应先补齐当前游戏的专属测试再汇报结果
- 测试结论必须和当前 `GamePackage`、Godot Host、Unreal 承接契约、Migration 文档保持一致
- 测试报告必须明确指出当前游戏的 GamePackage 是否已经被 Godot 与 Unreal 双宿主共同消费，而不是只验证单端实现

测试层级：
1. `GamePackage.Tests` 当前游戏的纯 .NET 测试（命令、状态、事件、回放、snapshot），禁止停留在模板样例测试
2. `standard-template/Core/SharedCore/` 与 `standard-template/Core/UiSchema/` 平台级测试
3. EngineBridge / Host Runtime 接口测试
4. Godot Host 集成测试（场景加载、输入转发、运行输出）
5. Unreal Host 集成测试（Actor 生成、UMG 绑定、运行输出）
6. 迁移验证测试（Godot → Unreal gameplay snapshot 对比）

筛选条件：$ARGUMENTS
