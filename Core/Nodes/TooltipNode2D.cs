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
        Visuals.Position = _offset;

        if (FollowMouse)
        {
            Visuals.Position = GetViewport().GetMousePosition();
        }

        if (!Visible)
        {
            return;
        }

        Vector2 finalPosition = Visuals.GetGlobalTransformWithCanvas().Origin + Visuals.Size;
        //TODO: This only works if the pivor is on top-left
        float xExtraAmount = GetViewport().GetVisibleRect().Size.X - finalPosition.X;
        if (xExtraAmount < 0)
        {
            Visuals.Position = new Vector2(Visuals.Position.X + xExtraAmount, Visuals.Position.Y);
        }
    }
}
