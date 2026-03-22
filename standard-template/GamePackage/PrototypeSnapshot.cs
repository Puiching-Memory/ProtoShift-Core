namespace ProtoShiftGame.GamePackage;

public sealed class PrototypeSnapshot
{
    public string Phase { get; set; } = string.Empty;
    public string Mode { get; set; } = string.Empty;
    public int ConfirmCount { get; set; }
    public string LastCommand { get; set; } = string.Empty;
    public string StatusText { get; set; } = string.Empty;
    public string DetailText { get; set; } = string.Empty;
    public string HintText { get; set; } = string.Empty;
    public IReadOnlyList<string> RecentEvents { get; set; } = Array.Empty<string>();
}