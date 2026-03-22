using ProtoShift.EngineBridge.Runtime;
using ProtoShiftGame.GamePackage;

namespace ProtoShiftGame.UnrealHost;

public sealed class TemplateUnrealHostCoordinator
{
    private readonly PrototypeRuntime _runtime = new();

    public PrototypeSnapshot Snapshot => _runtime.CaptureSnapshot();
    public PrototypeViewModel ViewModel => _runtime.CurrentViewModel;

    public void HandleAction(string actionName)
    {
        switch (actionName)
        {
            case "ConfirmPrimaryAction":
                _runtime.Dispatch(new PrototypeCommand(PrototypeCommandType.ConfirmPrimaryAction, "Confirmed"));
                break;
            case "TogglePause":
                _runtime.Dispatch(new PrototypeCommand(PrototypeCommandType.TogglePause));
                break;
            case "ResetDemo":
                _runtime.Dispatch(new PrototypeCommand(PrototypeCommandType.ResetDemo));
                break;
        }
    }

    public IReadOnlyList<HostSceneMutation> BuildInitialScenePlan()
    {
        return new[]
        {
            new HostSceneMutation("MainHud", "BindUiSchema", new Dictionary<string, string>
            {
                ["schema"] = "Migration/ui/template-hud.json",
            }),
        };
    }
}