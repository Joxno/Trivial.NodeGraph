using System.Numerics;

namespace Trivial.Domain.Geometry;

public record struct Line(Vector2 Start, Vector2 End);

public static class LineExtensions
{
    public static Vector2? GetIntersection(this Line Line, Line Other)
    {
        var t_Pt1Dir = new Vector2(Line.End.X - Line.Start.X, Line.End.Y - Line.Start.Y);
        var t_Pt2Dir = new Vector2(Other.End.X - Other.Start.X, Other.End.Y - Other.Start.Y);
        var t_Det = (t_Pt1Dir.X * t_Pt2Dir.Y) - (t_Pt1Dir.Y * t_Pt2Dir.X);
        var t_DeltaPt = new Vector2(Other.Start.X - Line.Start.X, Other.Start.Y - Line.Start.Y);
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

        return new Vector2(Line.Start.X + (t_Alpha * t_Pt1Dir.X / t_Det), Line.Start.Y + (t_Alpha * t_Pt1Dir.Y / t_Det));
    }

    public static string ToString(this Line L) => $"Line from {L.Start} to {L.End}";
}
