using Godot;

public partial class Main : Node
{
    private Label _statusLabel = null!;

    public override void _Ready()
    {
        _statusLabel = GetNode<Label>("UIRoot/Hud/StatusLabel");
        SetStatus("Template ready. Replace or extend this scaffold for your game.");
        GD.Print("ProtoShift standard Godot template initialized.");
    }

    public void SetStatus(string text)
    {
        _statusLabel.Text = text;
    }
}