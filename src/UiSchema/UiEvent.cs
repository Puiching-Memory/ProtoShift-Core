namespace ProtoShift.UiSchema;

/// <summary>
/// UI 事件类型。
/// </summary>
public enum UiEventType
{
    Click,
    Hover,
    Select,
    Disable,
    Focus,
    Blur
}

/// <summary>
/// UI 事件描述。
/// </summary>
public class UiEvent
{
    public UiEventType Type { get; set; }
    public string Action { get; set; } = string.Empty;
    public Dictionary<string, string> Parameters { get; set; } = new();
}
