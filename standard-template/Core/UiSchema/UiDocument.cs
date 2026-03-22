namespace ProtoShift.UiSchema;

/// <summary>
/// UI 描述文档根节点。
/// </summary>
public class UiDocument
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public UiTheme Theme { get; set; } = new();
    public UiNode Root { get; set; } = new();
    public Dictionary<string, UiBinding> Bindings { get; set; } = new();
}
