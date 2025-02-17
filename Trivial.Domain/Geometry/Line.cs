using System.Numerics;

namespace Trivial.Domain.Geometry;

public class Line
{
    public Line(Vector2 Start, Vector2 End)
    {
        this.Start = Start;
        this.End = End;
    }

    public Vector2 Start { get; }
    public Vector2 End { get; }

    public Vector2? GetIntersection(Line Line)
    {
        var t_Pt1Dir = new Vector2(End.X - Start.X, End.Y - Start.Y);
        var t_Pt2Dir = new Vector2(Line.End.X - Line.Start.X, Line.End.Y - Line.Start.Y);
        var t_Det = (t_Pt1Dir.X * t_Pt2Dir.Y) - (t_Pt1Dir.Y * t_Pt2Dir.X);
        var t_DeltaPt = new Vector2(Line.Start.X - Start.X, Line.Start.Y - Start.Y);
        var t_Alpha = (t_DeltaPt.X * t_Pt2Dir.Y) - (t_DeltaPt.Y * t_Pt2Dir.X);
        var t_Beta = (t_DeltaPt.X * t_Pt1Dir.Y) - (t_DeltaPt.Y * t_Pt1Dir.X);

        if (t_Det == 0 || t_Alpha * t_Det < 0 || t_Beta * t_Det < 0)
            return null;

        if (t_Det > 0)
        {
            if (t_Alpha > t_Det || t_Beta > t_Det)
                return null;

        }
        else
        {
            if (t_Alpha < t_Det || t_Beta < t_Det)
                return null;
        }

        return new Vector2(Start.X + (t_Alpha * t_Pt1Dir.X / t_Det), Start.Y + (t_Alpha * t_Pt1Dir.Y / t_Det));
    }

    public override string ToString() => $"Line from {Start} to {End}";
}
