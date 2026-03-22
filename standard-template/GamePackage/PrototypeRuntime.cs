using ProtoShift.EngineBridge.Runtime;

namespace ProtoShiftGame.GamePackage;

public sealed class PrototypeRuntime : IGameplayRuntime<PrototypeViewModel, PrototypeSnapshot>
{
    private readonly List<PrototypeEvent> _recentEvents = new();

    public string RuntimeId => "ProtoShiftGame.TemplateRuntime";
    public PrototypeState State { get; } = new();
    public PrototypeViewModel ViewModel { get; } = new();
    public PrototypeViewModel CurrentViewModel => ViewModel;

    public PrototypeRuntime()
    {
        Emit(PrototypeEventType.Bootstrapped, "Template runtime bootstrapped.");
        RefreshViewModel();
    }

    public void Dispatch(PrototypeCommand command)
    {
        switch (command.Type)
        {
            case PrototypeCommandType.ConfirmPrimaryAction:
                ConfirmPrimaryAction(command.Argument ?? "Confirmed");
                break;
            case PrototypeCommandType.TogglePause:
                TogglePause();
                break;
            case PrototypeCommandType.ResetDemo:
                ResetDemo();
                break;
        }
    }

    public PrototypeSnapshot CaptureSnapshot()
    {
        return new PrototypeSnapshot
        {
            Phase = State.Phase.ToString(),
            Mode = State.Mode,
            ConfirmCount = State.ConfirmCount,
            LastCommand = State.LastCommand,
            StatusText = ViewModel.StatusText,
            DetailText = ViewModel.DetailText,
            HintText = ViewModel.HintText,
            RecentEvents = _recentEvents.Select(x => x.Description).ToArray(),
        };
    }

    private void ConfirmPrimaryAction(string mode)
    {
        if (State.Phase == PrototypePhase.Paused)
            return;

        State.Phase = PrototypePhase.Running;
        State.Mode = mode;
        State.ConfirmCount += 1;
        State.LastCommand = PrototypeCommandType.ConfirmPrimaryAction.ToString();
        Emit(PrototypeEventType.ModeChanged, $"Mode switched to {mode}. Confirm count: {State.ConfirmCount}.");
        RefreshViewModel();
    }

    private void TogglePause()
    {
        if (State.Phase == PrototypePhase.Paused)
        {
            State.Phase = PrototypePhase.Running;
            State.LastCommand = PrototypeCommandType.TogglePause.ToString();
            Emit(PrototypeEventType.Resumed, "Template runtime resumed.");
        }
        else
        {
            State.Phase = PrototypePhase.Paused;
            State.LastCommand = PrototypeCommandType.TogglePause.ToString();
            Emit(PrototypeEventType.Paused, "Template runtime paused.");
        }

        RefreshViewModel();
    }

    private void ResetDemo()
    {
        State.Phase = PrototypePhase.Ready;
        State.Mode = "Template";
        State.ConfirmCount = 0;
        State.LastCommand = PrototypeCommandType.ResetDemo.ToString();
        Emit(PrototypeEventType.Reset, "Template runtime reset to initial state.");
        RefreshViewModel();
    }

    private void RefreshViewModel()
    {
        ViewModel.StatusText = State.Phase switch
        {
            PrototypePhase.Ready => "Template ready. Extend GamePackage before touching host code.",
            PrototypePhase.Running => $"Mode: {State.Mode}",
            PrototypePhase.Paused => "Template paused.",
            _ => "Template ready.",
        };

        ViewModel.DetailText = $"Last command: {State.LastCommand} | Confirm count: {State.ConfirmCount}";
        ViewModel.HintText = "Enter confirms a primary action, Esc pauses, R resets the demo.";
    }

    private void Emit(PrototypeEventType type, string description)
    {
        _recentEvents.Add(new PrototypeEvent(type, description));
        if (_recentEvents.Count > 5)
            _recentEvents.RemoveAt(0);
    }
}