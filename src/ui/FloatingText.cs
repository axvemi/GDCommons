using Godot;
using System;

namespace Axvemi.Commons;

public partial class FloatingText : RichTextLabel
{
    public enum FloatingType
    {
        Pop = 0,
        Float = 1
    }

    public const float AnimationDuration = 0.5f;

    public void Initialize(string text, Color color, Vector2 startingPosition, FloatingType type)
    {
        GlobalPosition = startingPosition;
        string bbColor = $"#{(int)(color.R * 255):X2}{(int)(color.G * 255):X2}{(int)(color.B * 255):X2}";
        Text = $"[color={bbColor}]{text}";

        switch (type)
        {
            case FloatingType.Float:
                StartFloatEffect();
                break;
            case FloatingType.Pop:
                StartPopEffect();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void StartFloatEffect()
    {
        Vector2 endPosition = GlobalPosition + new Vector2(0, -50);
        Tween tween = GetTree().CreateTween();

        tween.TweenProperty(this, "global_position", endPosition, 2.0f);
        tween.TweenCallback(new Callable(this, MethodName.QueueFree));
    }

    private void StartPopEffect()
    {
        Vector2 originalScale = Scale;
        Vector2 targetScale = originalScale * 2.0f;
        Vector2 endPosition = GlobalPosition + new Vector2(0, -50);

        Tween tween = GetTree().CreateTween();

        tween.TweenProperty(this, "scale", targetScale, 0.2f);
        tween.TweenProperty(this, "scale", originalScale, 0.2f);
        tween.TweenProperty(this, "global_position", endPosition, 1.0f);
        tween.TweenCallback(new Callable(this, MethodName.QueueFree));
    }
}
