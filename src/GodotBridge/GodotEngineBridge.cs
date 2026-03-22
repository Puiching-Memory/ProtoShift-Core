using System.Diagnostics;
using System.Text;
using System.Text.Json;
using ProtoShift.EngineBridge;

namespace ProtoShift.GodotBridge;

/// <summary>
/// Godot 引擎桥接实现。
/// 通过 CLI（godot --headless）和 MCP 协议控制 Godot 编辑器。
/// </summary>
public class GodotEngineBridge : EngineBridgeBase
{
    public override string EngineName => "Godot";

    private string? _projectPath;
    private Process? _runningProcess;

    public GodotEngineBridge(BackendConfig config) : base(config) { }

    public override async Task<BridgeResult> OpenProject(string projectPath)
    {
        _projectPath = projectPath;
        Log($"Open project: {projectPath}");

        if (!Directory.Exists(projectPath))
            return BridgeResult.Fail($"Project path not found: {projectPath}");

        var projectFile = Path.Combine(projectPath, "project.godot");
        if (!File.Exists(projectFile))
            return BridgeResult.Fail($"No project.godot found in: {projectPath}");

        return await Task.FromResult(BridgeResult.Ok($"Godot project opened: {projectPath}"));
    }

    public override async Task<BridgeResult> RunProject()
    {
        if (_projectPath == null)
            return BridgeResult.Fail("No project opened.");

        Log("Run project");
        return await ExecuteCli($"--path \"{_projectPath}\"");
    }

    public override async Task<BridgeResult> StopProject()
    {
        if (_runningProcess is { HasExited: false })
        {
            _runningProcess.Kill(entireProcessTree: true);
            _runningProcess = null;
            Log("Project stopped");
            return await Task.FromResult(BridgeResult.Ok("Project stopped."));
        }
        return await Task.FromResult(BridgeResult.Ok("No running process."));
    }

    public override async Task<BridgeResult> ReadLog(int lineCount = 100)
    {
        var lines = LogBuffer.TakeLast(lineCount).ToList();
        return await Task.FromResult(BridgeResult.Ok("Log retrieved", string.Join("\n", lines)));
    }

    public override async Task<BridgeResult> GetSceneTree(string scenePath)
    {
        if (_projectPath == null)
            return BridgeResult.Fail("No project opened.");

        var fullPath = Path.Combine(_projectPath, scenePath);
        if (!File.Exists(fullPath))
            return BridgeResult.Fail($"Scene not found: {scenePath}");

        var content = await File.ReadAllTextAsync(fullPath);
        Log($"Read scene tree: {scenePath}");
        return BridgeResult.Ok("Scene content loaded", content);
    }

    public override async Task<BridgeResult> CreateObject(string parentPath, string objectType, string objectName, Dictionary<string, string>? properties = null)
    {
        Log($"Create object: {objectName} ({objectType}) under {parentPath}");
        // 通过 Godot MCP 或 GDScript CLI 命令完成
        return await ExecuteGodotCommand("create_node", new
        {
            parent = parentPath,
            type = objectType,
            name = objectName,
            properties = properties ?? new Dictionary<string, string>()
        });
    }

    public override async Task<BridgeResult> ModifyProperty(string objectPath, string property, string value)
    {
        Log($"Modify property: {objectPath}.{property} = {value}");
        return await ExecuteGodotCommand("set_property", new
        {
            path = objectPath,
            property,
            value
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
        Log("Compile / reload Godot project");
        return await ExecuteCli($"--path \"{_projectPath}\" --headless --build-solutions --quit");
    }

    public override async Task<BridgeResult> QueryAssets(string filter)
    {
        if (_projectPath == null)
            return BridgeResult.Fail("No project opened.");

        var results = Directory.EnumerateFiles(_projectPath, filter, SearchOption.AllDirectories)
            .Select(f => Path.GetRelativePath(_projectPath, f))
            .ToList();

        return await Task.FromResult(BridgeResult.Ok($"Found {results.Count} assets",
            JsonSerializer.Serialize(results)));
    }

    public override async Task<BridgeResult> RunTests(string? filter = null)
    {
        Log($"Run Godot tests: {filter ?? "all"}");
        return await ExecuteCli($"--path \"{_projectPath}\" --headless --run-tests {filter ?? ""}".Trim());
    }

    public override async Task<BridgeResult> ExportMigrationModel(string outputPath)
    {
        Log($"Export migration model to: {outputPath}");
        // 读取 Godot 项目结构并序列化为迁移模型
        return await Task.FromResult(BridgeResult.Ok("Migration model export - stub", "{}"));
    }

    public override async Task<BridgeResult> ImportMigrationModel(string modelPath)
    {
        return await Task.FromResult(BridgeResult.Fail("Godot does not import migration models. Use UnrealBridge instead."));
    }

    // --- 内部工具 ---

    private async Task<BridgeResult> ExecuteCli(string arguments)
    {
        try
        {
            var godotPath = Config.Environment.GetValueOrDefault("GODOT_PATH", "godot");
            var psi = new ProcessStartInfo
            {
                FileName = godotPath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            foreach (var kv in Config.Environment)
                psi.Environment[kv.Key] = kv.Value;

            using var process = Process.Start(psi);
            if (process == null)
                return BridgeResult.Fail("Failed to start Godot process.");

            var stdout = await process.StandardOutput.ReadToEndAsync();
            var stderr = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
                return BridgeResult.Fail($"Godot CLI failed (exit {process.ExitCode}): {stderr}");

            return BridgeResult.Ok("CLI executed", stdout);
        }
        catch (Exception ex)
        {
            return BridgeResult.Fail($"CLI error: {ex.Message}");
        }
    }

    private async Task<BridgeResult> ExecuteGodotCommand(string command, object args)
    {
        // MCP 调用路径——当后端为 MCP 时发送 JSON-RPC
        if (Config.Kind == BackendKind.Mcp)
        {
            Log($"MCP call: {command}");
            // TODO: 实现 MCP JSON-RPC 客户端调用
            return await Task.FromResult(BridgeResult.Ok($"MCP stub: {command}"));
        }

        // CLI 回退路径
        var json = JsonSerializer.Serialize(args);
        return await ExecuteCli($"--path \"{_projectPath}\" --headless --command {command} --args {json}");
    }
}
