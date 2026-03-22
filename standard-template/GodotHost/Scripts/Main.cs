using Godot;
using ProtoShiftGame.GamePackage;

namespace ProtoShiftGame.GodotHost;

public partial class Main : Node
{
    private readonly PrototypeRuntime _runtime = new();
    private TemplateHudPresenter _hudPresenter = null!;

    public override void _Ready()
    {
        _hudPresenter = GetNode<TemplateHudPresenter>("UIRoot/Hud");
        ApplyViewModel();
        GD.Print("ProtoShift unified Godot template initialized.");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("confirm"))
        {
            _runtime.Dispatch(new PrototypeCommand(PrototypeCommandType.ConfirmPrimaryAction, "Confirmed"));
            ApplyViewModel();
        }

        if (@event.IsActionPressed("pause"))
        {
            _runtime.Dispatch(new PrototypeCommand(PrototypeCommandType.TogglePause));
            ApplyViewModel();
        }

        if (@event.IsActionPressed("reset_demo"))
        {
            _runtime.Dispatch(new PrototypeCommand(PrototypeCommandType.ResetDemo));
            ApplyViewModel();
        }
    }

    private void ApplyViewModel()
    {
        _hudPresenter.Apply(_runtime.ViewModel);
    }
}