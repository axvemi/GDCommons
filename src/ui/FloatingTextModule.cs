using Axvemi.Commons.Modules;
using Godot;
using System.Collections.Generic;
using static Axvemi.Commons.UI.FloatingText;

namespace Axvemi.Commons.UI;
public partial class FloatingTextModule<T> : Node, IModule<T>
{
    [Export] public Node2D FloatingTextSpawner { get; private set; }

    public ModuleController<T> ModuleController { get; set; }

    protected Vector2 StartingPosition;
    private Queue<FloatingTextCreationData> _creationQueue = new();
    private bool _canCreate = true;
    private PackedScene _floatingTextPrefab;


    public override void _Ready()
    {
        _floatingTextPrefab = GD.Load<PackedScene>("res://scenes/commons/floating_text.tscn");
    }

    public virtual void OnModulesReady()
    {
        StartingPosition = FloatingTextSpawner.GlobalPosition;
    }

    public void AddFloatingText(FloatingTextCreationData floatingTextCreationData)
    {
        _creationQueue.Enqueue(floatingTextCreationData);
        ProcessQueue();
    }

    private async void ProcessQueue()
    {
        while (_creationQueue.Count > 0)
        {
            if (!_canCreate)
            {
                await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);
                continue;
            }

            FloatingTextCreationData floatingTextCreationData = _creationQueue.Dequeue();
            FloatingText instance = (FloatingText)_floatingTextPrefab.Instantiate();

            GetTree().CurrentScene.AddChild(instance);
            instance.Initialize(floatingTextCreationData.Text, floatingTextCreationData.Color, StartingPosition, floatingTextCreationData.FloatingType);

            _canCreate = false;

            await ToSignal(GetTree().CreateTimer(FloatingText.AnimationDuration), SceneTreeTimer.SignalName.Timeout);

            _canCreate = true;
        }
    }
}

public class FloatingTextCreationData
{
    public string Text { get; }
    public Color Color { get; }
    public FloatingType FloatingType { get; }

    public FloatingTextCreationData(string text, Color color, FloatingType floatingType = FloatingType.Pop)
    {
        Text = text;
        Color = color;
        FloatingType = floatingType;
    }
}

