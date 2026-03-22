namespace ProtoShift.EngineBridge;

/// <summary>
/// 引擎后端类型。
/// </summary>
public enum BackendKind
{
    Mcp,
    Cli,
    Python,
    Plugin
}

/// <summary>
/// 引擎后端配置。
/// </summary>
public class BackendConfig
{
    public BackendKind Kind { get; set; } = BackendKind.Cli;
    public string Endpoint { get; set; } = string.Empty;
    public Dictionary<string, string> Environment { get; set; } = new();
    public int TimeoutMs { get; set; } = 30_000;
}

/// <summary>
/// 引擎桥接基类：每个引擎适配器继承此类实现 IEngineBridge。
/// 提供通用的后端调用和日志能力。
/// </summary>
public abstract class EngineBridgeBase : IEngineBridge
{
    public abstract string EngineName { get; }

    protected BackendConfig Config { get; }
    protected List<string> LogBuffer { get; } = new();

    protected EngineBridgeBase(BackendConfig config)
    {
        Config = config;
    }

    protected void Log(string message)
    {
        var entry = $"[{DateTime.UtcNow:HH:mm:ss}] [{EngineName}] {message}";
        LogBuffer.Add(entry);
    }

    // 子类必须实现
    public abstract Task<BridgeResult> OpenProject(string projectPath);
    public abstract Task<BridgeResult> RunProject();
    public abstract Task<BridgeResult> StopProject();
    public abstract Task<BridgeResult> ReadLog(int lineCount = 100);
    public abstract Task<BridgeResult> GetSceneTree(string scenePath);
    public abstract Task<BridgeResult> CreateObject(string parentPath, string objectType, string objectName, Dictionary<string, string>? properties = null);
    public abstract Task<BridgeResult> ModifyProperty(string objectPath, string property, string value);
    public abstract Task<BridgeResult> CreateScript(string path, string content);
    public abstract Task<BridgeResult> ReadScript(string path);
    public abstract Task<BridgeResult> CompileOrReload();
    public abstract Task<BridgeResult> QueryAssets(string filter);
    public abstract Task<BridgeResult> RunTests(string? filter = null);
    public abstract Task<BridgeResult> ExportMigrationModel(string outputPath);
    public abstract Task<BridgeResult> ImportMigrationModel(string modelPath);
}
