namespace ProtoShift.UiSchema;

/// <summary>
/// UI 样式定义。
/// </summary>
public class UiStyle
{
    // 尺寸
    public string? Width { get; set; }
    public string? Height { get; set; }
    public string? MinWidth { get; set; }
    public string? MinHeight { get; set; }
    public string? MaxWidth { get; set; }
    public string? MaxHeight { get; set; }

    // 间距
    public string? Margin { get; set; }
    public string? Padding { get; set; }
    public string? Gap { get; set; }

    // 排列
    public UiAlignment? Alignment { get; set; }
    public UiAlignment? CrossAlignment { get; set; }

    // 视觉
    public string? BackgroundColor { get; set; }
    public string? TextColor { get; set; }
    public string? BorderColor { get; set; }
    public string? BorderWidth { get; set; }
    public string? BorderRadius { get; set; }
    public string? FontSize { get; set; }
    public string? FontFamily { get; set; }

    // 可见性
    public bool Visible { get; set; } = true;
    public float Opacity { get; set; } = 1.0f;
}

public enum UiAlignment
{
    Start,
    Center,
    End,
    SpaceBetween,
    SpaceAround,
    Stretch
}
