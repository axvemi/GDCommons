using Godot;

namespace Axvemi.GDCommons.Tests;
public partial class TestFloatingText : Node
{
    [Export] protected PackedScene FloatingTextScene;
    protected Node2D Spawn;

    public override void _Ready()
    {
        base._Ready();
        Spawn = GetNode<Node2D>("Spawn");
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (Input.IsPhysicalKeyPressed(Key.Space))
        {
            var instance = FloatingTextScene.Instantiate() as FloatingText;
            Spawn.AddChild(instance);

            instance.Initialize("This is from the text", Colors.Purple);
            instance.Play();
        }
    }
}
