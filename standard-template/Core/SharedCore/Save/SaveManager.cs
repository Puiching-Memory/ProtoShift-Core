using System.Text.Json;

namespace ProtoShift.SharedCore.Save;

/// <summary>
/// 通用存档模型：序列化和反序列化游戏存档。
/// </summary>
public class SaveData
{
    public string Version { get; set; } = "1.0.0";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Dictionary<string, JsonElement> Sections { get; set; } = new();
}

/// <summary>
/// 存档管理器。
/// </summary>
public static class SaveManager
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
    };

    public static SaveData CreateNew()
    {
        return new SaveData { Timestamp = DateTime.UtcNow };
    }

    public static void SetSection<T>(SaveData save, string key, T data)
    {
        var element = JsonSerializer.SerializeToElement(data, Options);
        save.Sections[key] = element;
    }

    public static T? GetSection<T>(SaveData save, string key)
    {
        if (!save.Sections.TryGetValue(key, out var element))
            return default;
        return element.Deserialize<T>();
    }

    public static string Serialize(SaveData save)
    {
        save.Timestamp = DateTime.UtcNow;
        return JsonSerializer.Serialize(save, Options);
    }

    public static SaveData Deserialize(string json)
    {
        return JsonSerializer.Deserialize<SaveData>(json, Options) ?? CreateNew();
    }
}
