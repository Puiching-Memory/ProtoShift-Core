namespace ProtoShift.EngineBridge;

/// <summary>
/// 统一引擎操作结果。
/// </summary>
public class BridgeResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Data { get; set; }

    public static BridgeResult Ok(string message = "OK", string? data = null) =>
        new() { Success = true, Message = message, Data = data };

    public static BridgeResult Fail(string message) =>
        new() { Success = false, Message = message };
}

/// <summary>
/// 统一引擎桥接接口：对应 TODO 中的第一批统一语义。
/// Godot 和 Unreal 分别实现该接口。
/// </summary>
public interface IEngineBridge
{
    string EngineName { get; }

    // 项目管理
    Task<BridgeResult> OpenProject(string projectPath);
    Task<BridgeResult> RunProject();
    Task<BridgeResult> StopProject();
    Task<BridgeResult> ReadLog(int lineCount = 100);

    // 场景结构
    Task<BridgeResult> GetSceneTree(string scenePath);
    Task<BridgeResult> CreateObject(string parentPath, string objectType, string objectName, Dictionary<string, string>? properties = null);
    Task<BridgeResult> ModifyProperty(string objectPath, string property, string value);

    // 脚本
    Task<BridgeResult> CreateScript(string path, string content);
    Task<BridgeResult> ReadScript(string path);

    // 编译与热重载
    Task<BridgeResult> CompileOrReload();

    // 资源
    Task<BridgeResult> QueryAssets(string filter);

    // 测试
    Task<BridgeResult> RunTests(string? filter = null);

    // 迁移
    Task<BridgeResult> ExportMigrationModel(string outputPath);
    Task<BridgeResult> ImportMigrationModel(string modelPath);
}
