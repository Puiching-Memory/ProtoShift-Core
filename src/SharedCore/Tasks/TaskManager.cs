namespace ProtoShift.SharedCore.Tasks;

/// <summary>
/// 任务状态。
/// </summary>
public enum TaskStatus
{
    NotStarted,
    InProgress,
    Completed,
    Failed,
    Cancelled
}

/// <summary>
/// 通用任务节点，支持前置条件和完成回调。
/// </summary>
public class GameTask
{
    public string Id { get; }
    public string Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public TaskStatus Status { get; private set; } = TaskStatus.NotStarted;
    public List<string> Prerequisites { get; } = new();

    public event Action<GameTask>? OnCompleted;
    public event Action<GameTask>? OnFailed;

    public GameTask(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public void Start()
    {
        if (Status != TaskStatus.NotStarted) return;
        Status = TaskStatus.InProgress;
    }

    public void Complete()
    {
        if (Status != TaskStatus.InProgress) return;
        Status = TaskStatus.Completed;
        OnCompleted?.Invoke(this);
    }

    public void Fail()
    {
        if (Status != TaskStatus.InProgress) return;
        Status = TaskStatus.Failed;
        OnFailed?.Invoke(this);
    }

    public void Cancel()
    {
        if (Status is TaskStatus.Completed or TaskStatus.Failed) return;
        Status = TaskStatus.Cancelled;
    }
}

/// <summary>
/// 任务管理器：管理任务的注册、依赖和推进。
/// </summary>
public class TaskManager
{
    private readonly Dictionary<string, GameTask> _tasks = new();

    public IReadOnlyDictionary<string, GameTask> Tasks => _tasks;

    public GameTask Register(string id, string name)
    {
        var task = new GameTask(id, name);
        _tasks[id] = task;
        return task;
    }

    public bool CanStart(string taskId)
    {
        if (!_tasks.TryGetValue(taskId, out var task)) return false;
        return task.Prerequisites.TrueForAll(pid =>
            _tasks.TryGetValue(pid, out var prereq) && prereq.Status == TaskStatus.Completed);
    }

    public bool TryStart(string taskId)
    {
        if (!CanStart(taskId)) return false;
        _tasks[taskId].Start();
        return true;
    }
}
