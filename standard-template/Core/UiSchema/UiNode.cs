namespace ProtoShift.UiSchema;

/// <summary>
/// UI 节点类型枚举。
/// </summary>
public enum UiNodeType
{
    // 布局容器
    VBox,
    HBox,
    Grid,
    Scroll,
    Stack,

    // 基础组件
    Text,
    Image,
    Button,
    Panel,
    List,
    ProgressBar,
    Input,
}

/// <summary>
/// UI 节点：组成 UI 布局树的基本单元。
/// </summary>
public class UiNode
{
    public string Id { get; set; } = string.Empty;
    public UiNodeType Type { get; set; } = UiNodeType.Panel;
    public UiStyle Style { get; set; } = new();
    public Dictionary<string, string> Props { get; set; } = new();
    public List<UiNode> Children { get; set; } = new();
    public List<UiEvent> Events { get; set; } = new();
    public string? BindingKey { get; set; }
}
