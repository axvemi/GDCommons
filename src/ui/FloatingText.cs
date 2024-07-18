using Godot;

namespace Axvemi.Commons;

public partial class FloatingText : Node2D
{
    [Signal] public delegate void FinishedEventHandler(FloatingText floatingText);

    // OnReady
    public RichTextLabel Text;

    public float Duration { get; protected set; }

    public override void _Ready()
    {
        base._Ready();
        Text = GetNode<RichTextLabel>("%Text");

        Visible = false;
    }

    public void Initialize(string text, Color? color = null, float duration = 2)
    {
        Color colorToUse = color ?? Colors.Blue;
        Text.Text = $"[color={colorToUse.ToHtml()}]{text}";
        Duration = duration;
    }

    //TODO: Switch and enum/int to choose which effect to play
    public void Play()
    {
        PlayFloatEffect();
    }

    private void PlayFloatEffect()
    {
        Vector2 targetPosition = Position + new Vector2(0, -100);

        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(this, nameof(PropertyName.Position), targetPosition, Duration);
        tween.TweenCallback(new Callable(this, MethodName.QueueFree));
    }

    /*private void PlayPopEffect()
    {
        Vector2 originalScale = Scale;
        Vector2 targetScale = originalScale * 2.0f;
        Vector2 endPosition = Position + new Vector2(0, -50);

        Tween tween = GetTree().CreateTween();

        tween.TweenProperty(this, "scale", targetScale, 0.2f);
        tween.TweenProperty(this, "scale", originalScale, 0.2f);
        tween.TweenProperty(this, "global_position", endPosition, 1.0f);
        tween.TweenCallback(new Callable(this, MethodName.QueueFree));
    }*/
}
