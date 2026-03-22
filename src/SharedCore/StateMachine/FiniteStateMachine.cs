namespace ProtoShift.SharedCore.StateMachine;

/// <summary>
/// 有限状态机：管理状态、转换和回调。
/// 引擎无关，可被 Godot / Unreal 双端复用。
/// </summary>
public class FiniteStateMachine<TState> where TState : notnull
{
    private TState _current;
    private readonly Dictionary<TState, List<Transition<TState>>> _transitions = new();
    private readonly Dictionary<TState, Action?> _onEnter = new();
    private readonly Dictionary<TState, Action?> _onExit = new();

    public TState Current => _current;

    public event Action<TState, TState>? StateChanged;

    public FiniteStateMachine(TState initial)
    {
        _current = initial;
    }

    public void DefineState(TState state, Action? onEnter = null, Action? onExit = null)
    {
        _onEnter[state] = onEnter;
        _onExit[state] = onExit;
        if (!_transitions.ContainsKey(state))
            _transitions[state] = new List<Transition<TState>>();
    }

    public void AddTransition(TState from, TState to, Func<bool>? guard = null)
    {
        if (!_transitions.ContainsKey(from))
            _transitions[from] = new List<Transition<TState>>();
        _transitions[from].Add(new Transition<TState>(to, guard));
    }

    public bool TryTransition()
    {
        if (!_transitions.TryGetValue(_current, out var list))
            return false;

        foreach (var t in list)
        {
            if (t.Guard == null || t.Guard())
            {
                var prev = _current;
                _onExit.GetValueOrDefault(prev)?.Invoke();
                _current = t.Target;
                _onEnter.GetValueOrDefault(_current)?.Invoke();
                StateChanged?.Invoke(prev, _current);
                return true;
            }
        }
        return false;
    }

    public void ForceState(TState state)
    {
        var prev = _current;
        _onExit.GetValueOrDefault(prev)?.Invoke();
        _current = state;
        _onEnter.GetValueOrDefault(_current)?.Invoke();
        StateChanged?.Invoke(prev, _current);
    }
}

public sealed record Transition<TState>(TState Target, Func<bool>? Guard);
