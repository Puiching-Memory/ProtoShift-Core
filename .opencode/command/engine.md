---
name: engine
description: "执行统一引擎操作语义"
agent: protoshift
---

执行 ProtoShift 统一引擎操作。

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
- export <path> — 导出迁移模型
- import <path> — 导入迁移模型

命令：$ARGUMENTS
