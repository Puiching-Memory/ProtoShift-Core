using System.Text.Json;

namespace ProtoShift.SharedCore.Config;

/// <summary>
/// 通用配置加载器：从 JSON 文件读取强类型配置。
/// </summary>
public static class ConfigLoader
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    public static T Load<T>(string path) where T : new()
    {
        if (!File.Exists(path))
            return new T();
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<T>(json, Options) ?? new T();
    }

    public static T LoadFromString<T>(string json) where T : new()
    {
        return JsonSerializer.Deserialize<T>(json, Options) ?? new T();
    }

    public static void Save<T>(string path, T config)
    {
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions
        {
            WriteIndented = true,
        });
        File.WriteAllText(path, json);
    }
}
