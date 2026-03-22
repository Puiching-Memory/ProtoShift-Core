namespace ProtoShiftGame.GamePackage;

public enum PrototypeCommandType
{
    ConfirmPrimaryAction,
    TogglePause,
    ResetDemo,
}

public sealed record PrototypeCommand(PrototypeCommandType Type, string? Argument = null);