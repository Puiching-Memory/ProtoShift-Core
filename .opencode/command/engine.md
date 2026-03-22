---
name: engine
description: "执行统一引擎操作语义"
agent: protoshift
---

执行 ProtoShift 统一引擎操作。

这些操作服务于同一个跨引擎项目工作流，而不是把项目拆成独立的 Godot 阶段或 Unreal 阶段。

支持的操作：
- open <engine> <path> — 打开项目
- run <engine> — 运行项目
- stop <engine> — 停止运行
- log <engine> — 读取日志
- scene <engine> <path> — 获取场景结构
- create <engine> <parent> <type> <name> — 创建对象
- modify <engine> <path> <prop> <value> — 修改属性
- script <engine> create <path> — 创建脚本
- script <engine> read <path> — 读取脚本
- build <engine> — 编译或热重载
- assets <engine> <filter> — 查询资源
- export <path> — 导出或刷新双宿主共享产物（Scene Manifest、Gameplay Spec、UiSchema、host-only backlog）
- import <path> — 导入或核对双宿主共享产物

命令：$ARGUMENTS
