namespace ProtoShift.UiSchema;

/// <summary>
/// UI 主题令牌。
/// </summary>
public class UiTheme
{
    public Dictionary<string, string> Colors { get; set; } = new();
    public Dictionary<string, string> FontSizes { get; set; } = new();
    public Dictionary<string, string> FontFamilies { get; set; } = new();
    public Dictionary<string, string> Spacing { get; set; } = new();
    public Dictionary<string, string> BorderRadii { get; set; } = new();

    /// <summary>
    /// 创建一组合理的默认主题。
    /// </summary>
    public static UiTheme Default() => new()
    {
        Colors = new Dictionary<string, string>
        {
            ["primary"] = "#3B82F6",
            ["secondary"] = "#6B7280",
            ["background"] = "#1F2937",
            ["surface"] = "#374151",
            ["text"] = "#F9FAFB",
            ["textMuted"] = "#9CA3AF",
            ["success"] = "#10B981",
            ["warning"] = "#F59E0B",
            ["error"] = "#EF4444",
        },
        FontSizes = new Dictionary<string, string>
        {
            ["xs"] = "12",
            ["sm"] = "14",
            ["md"] = "16",
            ["lg"] = "20",
            ["xl"] = "24",
            ["xxl"] = "32",
        },
        Spacing = new Dictionary<string, string>
        {
            ["xs"] = "4",
            ["sm"] = "8",
            ["md"] = "16",
            ["lg"] = "24",
            ["xl"] = "32",
        },
        BorderRadii = new Dictionary<string, string>
        {
            ["none"] = "0",
            ["sm"] = "4",
            ["md"] = "8",
            ["lg"] = "16",
            ["full"] = "9999",
        },
    };
}
