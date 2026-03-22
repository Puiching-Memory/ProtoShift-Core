namespace ProtoShift.UiSchema;

/// <summary>
/// UI 绑定类型。
/// </summary>
public enum UiBindingType
{
    Text,
    Number,
    Visibility,
    List,
    Progress
}

/// <summary>
/// UI 数据绑定描述。
/// </summary>
public class UiBinding
{
    public string Key { get; set; } = string.Empty;
    public UiBindingType Type { get; set; } = UiBindingType.Text;

    /// <summary>ViewModel 中的属性路径。</summary>
    public string Source { get; set; } = string.Empty;

    /// <summary>可选的转换表达式。</summary>
    public string? Transform { get; set; }
}
