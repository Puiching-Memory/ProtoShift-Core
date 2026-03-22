---
name: migration
description: "迁移技能：管理 Godot 原型到 Unreal 5 的结构迁移"
---

# 迁移管理

## 能力

- 从 Godot 项目导出迁移模型
- 分析 SharedCore 共享逻辑覆盖率
- 生成 Unreal 侧承接结构
- 映射节点树到 Actor/Component 层级
- 映射 UiSchema 到 UMG
- 验证迁移完整性

## 迁移流程

```
Godot 项目
    ↓ 导出
迁移模型 (JSON)
    ├── 场景结构（节点树 → Actor 树）
    ├── 脚本引用（GDScript → 需重写 / C# → 直接复用）
    ├── 资源清单（材质、模型、音频）
    ├── SharedCore 引用列表
    └── UiSchema 引用列表
    ↓ 导入
Unreal 项目
    ├── Actor/Component 结构
    ├── UnrealSharp 项目引用 SharedCore
    ├── UMG Widget 映射
    └── 资源导入任务
```

## 迁移模型格式

```json
{
  "version": "1.0.0",
  "sourceEngine": "Godot",
  "targetEngine": "Unreal",
  "prototype": {
    "id": "...",
    "name": "...",
    "phase": "MigrationReady"
  },
  "scenes": [...],
  "scripts": {
    "shared": ["src/SharedCore/..."],
    "godotOnly": ["scripts/..."],
    "needsRewrite": ["scripts/..."]
  },
  "assets": [...],
  "uiDocuments": [...]
}
```

## 验证检查清单

- [ ] SharedCore 单元测试通过
- [ ] Unreal 项目编译成功
- [ ] UnrealSharp 正确加载 SharedCore DLL
- [ ] 场景结构对应关系完整
- [ ] UI 布局在 UMG 中可渲染
- [ ] 基础游玩流程可运行
