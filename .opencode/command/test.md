---
name: test
description: "运行 ProtoShift 分层测试"
agent: protoshift
---

运行 ProtoShift 分层测试套件。

测试层级：
1. SharedCore 单元测试（.NET 测试）
2. EngineBridge 接口测试
3. Godot 集成测试（场景加载、脚本执行）
4. Unreal 集成测试（Actor 生成、属性设置）
5. 迁移验证测试（Godot → Unreal 结构对比）

筛选条件：$ARGUMENTS
