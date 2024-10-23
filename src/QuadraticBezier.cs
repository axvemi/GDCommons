using System;
using Godot;

namespace Axvemi.Commons;

public class QuadraticBezier
{
    private readonly Vector2 _p0;
    private readonly Vector2 _p1;
    private readonly Vector2 _p2;

    public QuadraticBezier(Vector2 p0, Vector2 p1, Vector2 p2)
    {
        _p0 = p0;
        _p1 = p1;
        _p2 = p2;
    }

    public Vector2 GetValue(float t)
    {
        if (t is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(t), "t must be between 0 and 1");
        }
        Vector2 q0 = _p0.Lerp(_p1, t);
        Vector2 q1 = _p1.Lerp(_p2, t);

        return q0.Lerp(q1, t);
    }

}