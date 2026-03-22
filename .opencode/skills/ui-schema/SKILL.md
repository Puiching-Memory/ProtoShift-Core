---
name: ui-schema
description: "UI Schema 技能：管理通用 UI 描述、主题和双端映射"
---

# UI Schema 管理

## 能力

- 定义 UI 布局树（UiDocument → UiNode 层级）
- 配置主题令牌（颜色、字号、间距、圆角）
- 设置数据绑定（文本、数值、可见性、列表）
- 定义交互事件（点击、悬停、焦点）
- 管理 ViewModel 合约，保持其由 `GamePackage` 输出
- 生成 Godot Control 映射提示
- 生成 Unreal UMG 映射提示
- 检查 Godot 与 Unreal UI 映射是否仍然消费当前项目的真实 ViewModel，而不是模板残留字段

## 项目位置

`standard-template/Core/UiSchema/` — .NET 8 类库

项目级 UI 产物建议放在 `workspace/<project-slug>/Migration/ui/`，并由 `GamePackage` 提供 ViewModel。

UiSchema 不是后期补文档；它是从原型第一天起就要同时服务 Godot HUD 和 Unreal UMG 的共享契约。

## 组件类型

| UiNodeType | 说明 | Godot | Unreal |
|------------|------|-------|--------|
| VBox | 纵向布局 | VBoxContainer | VerticalBox |
| HBox | 横向布局 | HBoxContainer | HorizontalBox |
| Grid | 网格布局 | GridContainer | UniformGridPanel |
| Scroll | 滚动容器 | ScrollContainer | ScrollBox |
| Stack | 叠层容器 | Control (z-order) | Overlay |
| Text | 文本 | Label / RichTextLabel | TextBlock |
| Image | 图片 | TextureRect | Image |
| Button | 按钮 | Button | Button |
| Panel | 面板 | Panel / PanelContainer | Border |
| List | 列表 | ItemList / VBoxContainer | ListView |
| ProgressBar | 进度条 | ProgressBar | ProgressBar |
| Input | 输入框 | LineEdit | EditableText |

## JSON 格式示例

```json
{
  "id": "main-hud",
  "name": "Main HUD",
  "theme": {
    "colors": { "primary": "#3B82F6", "text": "#F9FAFB" },
    "fontSizes": { "md": "16" },
    "spacing": { "md": "16" }
  },
  "root": {
    "id": "root",
    "type": "VBox",
    "style": { "padding": "16", "gap": "8" },
    "children": [
      {
        "id": "health-bar",
        "type": "ProgressBar",
        "bindingKey": "playerHealth",
        "style": { "width": "200", "height": "20" }
      },
      {
        "id": "score-text",
        "type": "Text",
        "props": { "text": "Score: 0" },
        "bindingKey": "playerScore"
      }
    ]
  },
  "bindings": {
    "playerHealth": { "key": "playerHealth", "type": "Progress", "source": "player.health" },
    "playerScore": { "key": "playerScore", "type": "Text", "source": "player.score" }
  }
}
```
