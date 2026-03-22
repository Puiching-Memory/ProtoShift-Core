namespace ProtoShiftGame.GamePackage;

public enum PrototypeEventType
{
    Bootstrapped,
    ModeChanged,
    Paused,
    Resumed,
    Reset,
}

public sealed record PrototypeEvent(PrototypeEventType Type, string Description);