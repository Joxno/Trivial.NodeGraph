using System;
using System.Collections.Generic;
using System.Numerics;
using Trivial.Domain.Extensions;

namespace Trivial.Domain.Geometry;

public class Ellipse : IShape
{
    public Ellipse(float cx, float cy, float rx, float ry)
    {
        Cx = cx;
        Cy = cy;
        Rx = rx;
        Ry = ry;
    }

    public float Cx { get; }
    public float Cy { get; }
    public float Rx { get; }
    public float Ry { get; }

    public IEnumerable<Vector2> GetIntersectionsWithLine(Line line)
    {
        var a1 = line.Start;
        var a2 = line.End;
        var dir = new Vector2(line.End.X - line.Start.X, line.End.Y - line.Start.Y);
        var diff = a1 - new Vector2(Cx, Cy);
        var mDir = new Vector2(dir.X / (Rx * Rx), dir.Y / (Ry * Ry));
        var mDiff = new Vector2(diff.X / (Rx * Rx), diff.Y / (Ry * Ry));

        var a = dir.Dot(mDir);
        var b = dir.Dot(mDiff);
        var c = diff.Dot(mDiff) - 1.0f;
        var d = b * b - a * c;

        if (d > 0)
        {
            var root = MathF.Sqrt(d);
            var ta = (-b - root) / a;
            var tb = (-b + root) / a;

            if (ta >= 0 && 1 >= ta || tb >= 0 && 1 >= tb)
            {
                if (0 <= ta && ta <= 1)
                    yield return a1.Lerp(a2, ta);

                if (0 <= tb && tb <= 1)
                    yield return a1.Lerp(a2, tb);
            }
        }
        else
        {
            var t = -b / a;
            if (0 <= t && t <= 1)
            {
                yield return a1.Lerp(a2, t);
            }
        }
    }

    public Vector2? GetPointAtAngle(float a)
    {
        var t = MathF.Tan(a / 360 * MathF.PI);
        var px = Rx * (1 - MathF.Pow(t, 2)) / (1 + MathF.Pow(t, 2));
        var py = Ry * 2 * t / (1 + MathF.Pow(t, 2));
        return new Vector2(Cx + px, Cy + py);
    }
}
