using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using ProtoShift.EngineBridge;

namespace ProtoShift.UnrealBridge;

/// <summary>
/// Unreal 引擎桥接实现。
/// 支持通过 Remote Control API、CLI 和 MCP 协议控制 Unreal 编辑器。
/// </summary>
public class UnrealEngineBridge : EngineBridgeBase
{
    public override string EngineName => "Unreal";

    private string? _projectPath;
    private readonly HttpClient _http;

    public UnrealEngineBridge(BackendConfig config) : base(config)
    {
        _http = new HttpClient();
        if (!string.IsNullOrEmpty(config.Endpoint))
            _http.BaseAddress = new Uri(config.Endpoint);
    }

    public override async Task<BridgeResult> OpenProject(string projectPath)
    {
        _projectPath = projectPath;
        Log($"Open project: {projectPath}");

        if (!Directory.Exists(projectPath))
            return BridgeResult.Fail($"Project path not found: {projectPath}");

        var uprojectFiles = Directory.GetFiles(projectPath, "*.uproject");
        if (uprojectFiles.Length == 0)
            return BridgeResult.Fail($"No .uproject found in: {projectPath}");

        return await Task.FromResult(BridgeResult.Ok($"Unreal project opened: {projectPath}"));
    }

    public override async Task<BridgeResult> RunProject()
    {
        if (_projectPath == null)
            return BridgeResult.Fail("No project opened.");

        Log("Run Unreal project (PIE)");
        return await CallRemoteControl("/remote/object/call", new
        {
            objectPath = "/Script/UnrealEd.Default__UnrealEditorSubsystem",
            functionName = "PIE_Play"
        });
    }

    public override async Task<BridgeResult> StopProject()
    {
        Log("Stop Unreal project (PIE)");
        return await CallRemoteControl("/remote/object/call", new
        {
            objectPath = "/Script/UnrealEd.Default__UnrealEditorSubsystem",
            functionName = "PIE_Stop"
        });
    }

    public override async Task<BridgeResult> ReadLog(int lineCount = 100)
    {
        var lines = LogBuffer.TakeLast(lineCount).ToList();
        return await Task.FromResult(BridgeResult.Ok("Log retrieved", string.Join("\n", lines)));
    }

    public override async Task<BridgeResult> GetSceneTree(string scenePath)
    {
        Log($"Get scene tree: {scenePath}");
        return await CallRemoteControl("/remote/object/property", new
        {
            objectPath = scenePath,
            propertyName = "RootComponent"
        });
    }

    public override async Task<BridgeResult> CreateObject(string parentPath, string objectType, string objectName, Dictionary<string, string>? properties = null)
    {
        Log($"Create object: {objectName} ({objectType}) under {parentPath}");
        return await CallRemoteControl("/remote/object/call", new
        {
            objectPath = "/Script/UnrealEd.Default__EditorLevelLibrary",
            functionName = "SpawnActorFromClass",
            parameters = new
            {
                ActorClass = objectType,
                Location = new { X = 0, Y = 0, Z = 0 }
            }
        });
    }

    public override async Task<BridgeResult> ModifyProperty(string objectPath, string property, string value)
    {
        Log($"Modify property: {objectPath}.{property} = {value}");
        return await CallRemoteControl("/remote/object/property", new
        {
            objectPath,
            propertyName = property,
            propertyValue = JsonSerializer.Deserialize<JsonElement>($"\"{value}\"")
        });
    }

    public override async Task<BridgeResult> CreateScript(string path, string content)
    {
        if (_projectPath == null)
            return BridgeResult.Fail("No project opened.");

        var fullPath = Path.Combine(_projectPath, path);
        var dir = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        await File.WriteAllTextAsync(fullPath, content);
        Log($"Script created: {path}");
        return BridgeResult.Ok($"Script created: {path}");
    }

    public override async Task<BridgeResult> ReadScript(string path)
    {
        if (_projectPath == null)
            return BridgeResult.Fail("No project opened.");

        var fullPath = Path.Combine(_projectPath, path);
        if (!File.Exists(fullPath))
            return BridgeResult.Fail($"Script not found: {path}");

        var content = await File.ReadAllTextAsync(fullPath);
        return BridgeResult.Ok("Script content loaded", content);
    }

    public override async Task<BridgeResult> CompileOrReload()
    {
        Log("Compile / hot reload Unreal project");
        return await CallRemoteControl("/remote/object/call", new
        {
            objectPath = "/Script/UnrealEd.Default__UnrealEditorSubsystem",
            functionName = "CompileBlueprints"
        });
    }

    public override async Task<BridgeResult> QueryAssets(string filter)
    {
        Log($"Query assets: {filter}");
        return await CallRemoteControl("/remote/object/call", new
        {
            objectPath = "/Script/UnrealEd.Default__EditorAssetLibrary",
            functionName = "ListAssets",
            parameters = new { path = filter }
        });
    }

    public override async Task<BridgeResult> RunTests(string? filter = null)
    {
        Log($"Run Unreal tests: {filter ?? "all"}");
        var args = $"--path \"{_projectPath}\" -ExecCmds=\"Automation RunTests {filter ?? ""}\"";
        return await ExecuteUat(args);
    }

    public override async Task<BridgeResult> ExportMigrationModel(string outputPath)
    {
        return await Task.FromResult(BridgeResult.Fail("Use GodotBridge to export migration model."));
    }

    public override async Task<BridgeResult> ImportMigrationModel(string modelPath)
    {
        Log($"Import migration model from: {modelPath}");
        if (!File.Exists(modelPath))
            return BridgeResult.Fail($"Model not found: {modelPath}");

        var json = await File.ReadAllTextAsync(modelPath);
        // TODO: 解析迁移模型并通过 Remote Control / UnrealSharp 生成对应结构
        Log("Migration model imported (stub)");
        return BridgeResult.Ok("Migration model imported", json);
    }

    // --- 内部工具 ---

    private async Task<BridgeResult> CallRemoteControl(string path, object body)
    {
        try
        {
            var endpoint = Config.Endpoint;
            if (string.IsNullOrEmpty(endpoint))
                endpoint = "http://127.0.0.1:30010";

            var url = $"{endpoint.TrimEnd('/')}{path}";
            var response = await _http.PutAsJsonAsync(url, body);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return BridgeResult.Fail($"Remote Control failed ({response.StatusCode}): {content}");

            return BridgeResult.Ok("Remote Control call succeeded", content);
        }
        catch (Exception ex)
        {
            return BridgeResult.Fail($"Remote Control error: {ex.Message}");
        }
    }

    private async Task<BridgeResult> ExecuteUat(string arguments)
    {
        try
        {
            var uatPath = Config.Environment.GetValueOrDefault("UAT_PATH", "RunUAT.bat");
            var psi = new ProcessStartInfo
            {
                FileName = uatPath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using var process = Process.Start(psi);
            if (process == null)
                return BridgeResult.Fail("Failed to start UAT process.");

            var stdout = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            return process.ExitCode == 0
                ? BridgeResult.Ok("UAT executed", stdout)
                : BridgeResult.Fail($"UAT failed (exit {process.ExitCode}): {stdout}");
        }
        catch (Exception ex)
        {
            return BridgeResult.Fail($"UAT error: {ex.Message}");
        }
    }
}
