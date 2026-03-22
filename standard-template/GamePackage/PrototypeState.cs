namespace ProtoShiftGame.GamePackage;

public enum PrototypePhase
{
    Ready,
    Running,
    Paused,
}

public sealed class PrototypeState
{
    public PrototypePhase Phase { get; set; } = PrototypePhase.Ready;
    public string Mode { get; set; } = "Template";
    public int ConfirmCount { get; set; }
    public string LastCommand { get; set; } = "Boot";
}

public sealed class PrototypeViewModel
{
    public string StatusText { get; set; } = string.Empty;
    public string DetailText { get; set; } = string.Empty;
    public string HintText { get; set; } = string.Empty;
}