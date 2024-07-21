using Axvemi.Commons;
using Godot;
using System.Collections.Generic;

namespace Axvemi.GDCommons;
public partial class FloatingTextModule<T> : Node, IModule<T>
{
    public ModuleController<T> ModuleController { get; set; }

    [Export] public PackedScene FloatingTextScene;

    public float TimeBetweenTexts { get; protected set; } = 0.6f;
    public Queue<FloatingText> Queue = new();
    private float _remainingTime;

    public override void _Ready()
    {
    }

    public virtual void OnModulesReady()
    {
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (_remainingTime >= 0)
        {
            _remainingTime -= (float)delta;
            return;
        }
        if (Queue.Count == 0)
        {
            return;
        }

        FloatingText currentText = Queue.Dequeue();
        currentText.Play();
        _remainingTime = TimeBetweenTexts;
    }
}
