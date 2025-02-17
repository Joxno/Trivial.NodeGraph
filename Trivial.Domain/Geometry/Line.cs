using System.Numerics;

namespace Trivial.Domain.Geometry;

public class Line
{
    public Line(Vector2 start, Vector2 end)
    {
        Start = start;
        End = end;
    }

    public Vector2 Start { get; }
    public Vector2 End { get; }

    public Vector2? GetIntersection(Line line)
    {
        var pt1Dir = new Vector2(End.X - Start.X, End.Y - Start.Y);
        var pt2Dir = new Vector2(line.End.X - line.Start.X, line.End.Y - line.Start.Y);
        var det = (pt1Dir.X * pt2Dir.Y) - (pt1Dir.Y * pt2Dir.X);
        var deltaPt = new Vector2(line.Start.X - Start.X, line.Start.Y - Start.Y);
        var alpha = (deltaPt.X * pt2Dir.Y) - (deltaPt.Y * pt2Dir.X);
        var beta = (deltaPt.X * pt1Dir.Y) - (deltaPt.Y * pt1Dir.X);

        if (det == 0 || alpha * det < 0 || beta * det < 0)
            return null;

        if (det > 0)
        {
            if (alpha > det || beta > det)
                return null;

        }
        else
        {
            if (alpha < det || beta < det)
                return null;
        }

        return new Vector2(Start.X + (alpha * pt1Dir.X / det), Start.Y + (alpha * pt1Dir.Y / det));
    }

    public override string ToString() => $"Line from {Start} to {End}";
}
