namespace ProtoShift.EngineBridge;

/// <summary>
/// 引擎注册表：管理已注册的引擎桥接。
/// </summary>
public class EngineRegistry
{
    private readonly Dictionary<string, IEngineBridge> _bridges = new(StringComparer.OrdinalIgnoreCase);

    public void Register(IEngineBridge bridge)
    {
        _bridges[bridge.EngineName] = bridge;
    }

    public IEngineBridge? Get(string engineName)
    {
        _bridges.TryGetValue(engineName, out var bridge);
        return bridge;
    }

    public IEnumerable<string> RegisteredEngines => _bridges.Keys;

    public IEngineBridge Require(string engineName)
    {
        return Get(engineName) ?? throw new InvalidOperationException($"Engine '{engineName}' is not registered.");
    }
}
