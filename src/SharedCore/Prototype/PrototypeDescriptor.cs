namespace ProtoShift.SharedCore.Prototype;

/// <summary>
/// 原型描述模型：用于描述一个游戏原型的基本元数据，
/// 可在 Godot 和 Unreal 之间迁移。
/// </summary>
public class PrototypeDescriptor
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public PrototypePhase Phase { get; set; } = PrototypePhase.Concept;

    /// <summary>场景/关卡列表。</summary>
    public List<SceneDescriptor> Scenes { get; set; } = new();

    /// <summary>共享规则引用。</summary>
    public List<string> SharedRuleIds { get; set; } = new();

    /// <summary>额外元数据。</summary>
    public Dictionary<string, string> Metadata { get; set; } = new();
}

public enum PrototypePhase
{
    Concept,
    GodotPrototype,
    GodotStable,
    MigrationReady,
    UnrealInProgress,
    UnrealStable
}

/// <summary>
/// 场景描述。
/// </summary>
public class SceneDescriptor
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public SceneType Type { get; set; } = SceneType.Level;
    public List<ObjectDescriptor> Objects { get; set; } = new();
}

public enum SceneType
{
    Level,
    UI,
    Prefab,
    Test
}

/// <summary>
/// 对象描述：场景中的实体。
/// </summary>
public class ObjectDescriptor
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string TypeHint { get; set; } = string.Empty;
    public Dictionary<string, string> Properties { get; set; } = new();
    public List<ObjectDescriptor> Children { get; set; } = new();
}
