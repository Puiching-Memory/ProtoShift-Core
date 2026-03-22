using ProtoShift.SharedCore.StateMachine;
using ProtoShift.SharedCore.Tasks;

namespace ProtoShift.Samples.SharedCoreUsage;

/// <summary>
/// 演示 SharedCore 的基本使用方式。
/// 这段代码在 Godot (C#) 和 Unreal (UnrealSharp) 中均可运行。
/// </summary>
public static class PoolGameExample
{
    public enum GamePhase
    {
        WaitingForPlayers,
        Player1Turn,
        Player2Turn,
        BallsMoving,
        Foul,
        GameOver
    }

    public static void Run()
    {
        // --- 状态机示例 ---
        var fsm = new FiniteStateMachine<GamePhase>(GamePhase.WaitingForPlayers);

        fsm.DefineState(GamePhase.WaitingForPlayers,
            onEnter: () => Console.WriteLine("等待玩家加入..."));
        fsm.DefineState(GamePhase.Player1Turn,
            onEnter: () => Console.WriteLine("玩家 1 回合"));
        fsm.DefineState(GamePhase.Player2Turn,
            onEnter: () => Console.WriteLine("玩家 2 回合"));
        fsm.DefineState(GamePhase.BallsMoving,
            onEnter: () => Console.WriteLine("球正在移动..."));
        fsm.DefineState(GamePhase.GameOver,
            onEnter: () => Console.WriteLine("游戏结束"));

        var gameStarted = false;
        fsm.AddTransition(GamePhase.WaitingForPlayers, GamePhase.Player1Turn,
            guard: () => gameStarted);
        fsm.AddTransition(GamePhase.Player1Turn, GamePhase.BallsMoving);
        fsm.AddTransition(GamePhase.Player2Turn, GamePhase.BallsMoving);

        fsm.StateChanged += (from, to) =>
            Console.WriteLine($"状态变更: {from} → {to}");

        // 模拟游戏开始
        gameStarted = true;
        fsm.TryTransition(); // WaitingForPlayers → Player1Turn

        // --- 任务系统示例 ---
        var taskMgr = new TaskManager();

        var setup = taskMgr.Register("setup-table", "摆放球");
        var practice = taskMgr.Register("practice-shot", "练习击球");
        practice.Prerequisites.Add("setup-table");

        var match = taskMgr.Register("start-match", "开始比赛");
        match.Prerequisites.Add("practice-shot");

        // 按依赖顺序推进
        taskMgr.TryStart("setup-table");
        setup.Complete();

        taskMgr.TryStart("practice-shot");
        practice.Complete();

        taskMgr.TryStart("start-match");

        Console.WriteLine($"比赛任务状态: {match.Status}");
    }
}
