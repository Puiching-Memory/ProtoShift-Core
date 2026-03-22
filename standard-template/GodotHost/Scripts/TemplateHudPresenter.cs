using Godot;
using ProtoShiftGame.GamePackage;

namespace ProtoShiftGame.GodotHost;

public partial class TemplateHudPresenter : Control
{
    private Label _statusLabel = null!;
    private Label _detailLabel = null!;
    private Label _hintLabel = null!;

    public override void _Ready()
    {
        _statusLabel = GetNode<Label>("StatusLabel");
        _detailLabel = GetNode<Label>("DetailLabel");
        _hintLabel = GetNode<Label>("HintLabel");
    }

    public void Apply(PrototypeViewModel viewModel)
    {
        _statusLabel.Text = viewModel.StatusText;
        _detailLabel.Text = viewModel.DetailText;
        _hintLabel.Text = viewModel.HintText;
    }
}