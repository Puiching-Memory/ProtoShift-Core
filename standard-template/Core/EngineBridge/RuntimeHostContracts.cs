using System.Numerics;

namespace ProtoShift.EngineBridge.Runtime;

public readonly record struct HostActionState(string Name, bool Pressed, float Value = 0f);

public readonly record struct HostInputSnapshot(Vector2 PointerPosition, IReadOnlyList<HostActionState> Actions);

public readonly record struct HostBodyState(string Id, Vector2 Position, Vector2 LinearVelocity, bool Visible);

public readonly record struct HostSceneMutation(string TargetId, string Operation, IReadOnlyDictionary<string, string>? Properties = null);

public interface IHostClock
{
    double DeltaSeconds { get; }
    ulong FrameIndex { get; }
}

public interface IHostInputSource
{
    HostInputSnapshot CaptureInput();
}

public interface IHostPhysicsSource
{
    IReadOnlyList<HostBodyState> CaptureBodies();
}

public interface IHostSceneSink
{
    Task ApplySceneMutations(IReadOnlyList<HostSceneMutation> mutations, CancellationToken cancellationToken = default);
}

public interface IHostUiSink
{
    Task ApplyViewModel<TViewModel>(TViewModel viewModel, CancellationToken cancellationToken = default);
}

public interface IGameplayRuntime<out TViewModel, out TSnapshot>
{
    string RuntimeId { get; }
    TViewModel CurrentViewModel { get; }
    TSnapshot CaptureSnapshot();
}

public interface IGameRuntimeHost
{
    IHostClock Clock { get; }
    IHostInputSource Input { get; }
    IHostPhysicsSource Physics { get; }
    IHostSceneSink Scene { get; }
    IHostUiSink Ui { get; }
}