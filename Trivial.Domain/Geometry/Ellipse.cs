using System;
using System.Collections.Generic;
using System.Numerics;
using Trivial.Graph.Domain.Extensions;

namespace Trivial.Graph.Domain.Geometry;

public class Ellipse : IShape
{
    public Ellipse(float Cx, float Cy, float Rx, float Ry)
    {
        this.Cx = Cx;
        this.Cy = Cy;
        this.Rx = Rx;
        this.Ry = Ry;
    }

    public float Cx { get; }
    public float Cy { get; }
    public float Rx { get; }
    public float Ry { get; }

    public IEnumerable<Vector2> GetIntersectionsWithLine(Line Line)
    {
        var t_A1 = Line.Start;
        var t_A2 = Line.End;
        var t_Dir = new Vector2(Line.End.X - Line.Start.X, Line.End.Y - Line.Start.Y);
        var t_Diff = t_A1 - new Vector2(Cx, Cy);
        var t_MDir = new Vector2(t_Dir.X / (Rx * Rx), t_Dir.Y / (Ry * Ry));
        var t_MDiff = new Vector2(t_Diff.X / (Rx * Rx), t_Diff.Y / (Ry * Ry));

        var t_A = t_Dir.Dot(t_MDir);
        var t_B = t_Dir.Dot(t_MDiff);
        var t_C = t_Diff.Dot(t_MDiff) - 1.0f;
        var t_D = t_B * t_B - t_A * t_C;

        if (t_D > 0)
        {
            var t_Root = MathF.Sqrt(t_D);
            var t_Ta = (-t_B - t_Root) / t_A;
            var t_Tb = (-t_B + t_Root) / t_A;

            if (t_Ta >= 0 && 1 >= t_Ta || t_Tb >= 0 && 1 >= t_Tb)
            {
                if (0 <= t_Ta && t_Ta <= 1)
                    yield return t_A1.Lerp(t_A2, t_Ta);

                if (0 <= t_Tb && t_Tb <= 1)
                    yield return t_A1.Lerp(t_A2, t_Tb);
            }
        }
        else
        {
            var t_T = -t_B / t_A;
            if (0 <= t_T && t_T <= 1)
            {
                yield return t_A1.Lerp(t_A2, t_T);
            }
        }
    }

    public Vector2? GetPointAtAngle(float A)
    {
        var t_T = MathF.Tan(A / 360 * MathF.PI);
        var t_Px = Rx * (1 - MathF.Pow(t_T, 2)) / (1 + MathF.Pow(t_T, 2));
        var t_Py = Ry * 2 * t_T / (1 + MathF.Pow(t_T, 2));
        return new Vector2(Cx + t_Px, Cy + t_Py);
    }
}
