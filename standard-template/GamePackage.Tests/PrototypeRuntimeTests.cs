using System.Text.Json;
using System.Text.Json.Nodes;
using ProtoShiftGame.GamePackage;
using Xunit;

namespace ProtoShiftGame.GamePackage.Tests;

public sealed class PrototypeRuntimeTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    [Fact]
    public void Initial_State_Matches_Baseline()
    {
        var runtime = new PrototypeRuntime();
        AssertSnapshot("initial-state.json", runtime.CaptureSnapshot());
    }

    [Fact]
    public void Confirm_Action_Updates_Baseline()
    {
        var runtime = new PrototypeRuntime();
        runtime.Dispatch(new PrototypeCommand(PrototypeCommandType.ConfirmPrimaryAction, "Confirmed"));
        AssertSnapshot("confirmed-state.json", runtime.CaptureSnapshot());
    }

    [Fact]
    public void Pause_Action_Updates_Baseline()
    {
        var runtime = new PrototypeRuntime();
        runtime.Dispatch(new PrototypeCommand(PrototypeCommandType.TogglePause));
        AssertSnapshot("paused-state.json", runtime.CaptureSnapshot());
    }

    [Fact]
    public void Reset_Action_Restores_Ready_State_With_Reset_Metadata()
    {
        var runtime = new PrototypeRuntime();
        runtime.Dispatch(new PrototypeCommand(PrototypeCommandType.ConfirmPrimaryAction, "Confirmed"));
        runtime.Dispatch(new PrototypeCommand(PrototypeCommandType.ResetDemo));
        AssertSnapshot("reset-state.json", runtime.CaptureSnapshot());
    }

    private static void AssertSnapshot(string fileName, PrototypeSnapshot snapshot)
    {
        var snapshotPath = Path.Combine(AppContext.BaseDirectory, "snapshots", fileName);
        var expected = JsonNode.Parse(File.ReadAllText(snapshotPath));
        var actual = JsonNode.Parse(JsonSerializer.Serialize(snapshot, JsonOptions));

        Assert.True(JsonNode.DeepEquals(expected, actual),
            $"Snapshot mismatch for {fileName}\nExpected:\n{expected}\nActual:\n{actual}");
    }
}