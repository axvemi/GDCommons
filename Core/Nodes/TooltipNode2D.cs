using Godot;

namespace Axvemi.Commons;

[GlobalClass]
public partial class TooltipNode2D : Node2D
{
    [Export] public bool FollowMouse;
    [Export] private PackedScene _visualsPrefab;
    [Export] private Vector2 _offset;

    public Control Visuals { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        Visible = false;

        Visuals = (Control)_visualsPrefab.Instantiate();
        AddChild(Visuals);

        Visuals.Position = _offset;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (!Visible)
        {
            return;
        }



        if (!FollowMouse)
        {
            return;
        }

        Visuals.Position = GetViewport().GetMousePosition();
    }
}
