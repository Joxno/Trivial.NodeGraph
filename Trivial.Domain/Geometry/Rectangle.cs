using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Trivial.Graph.Domain.Geometry;

public class Rectangle : IShape
{
    public static Rectangle Zero { get; } = new(0, 0, 0, 0);

    public float Width { get; }
    public float Height { get; }
    public float Top { get; }
    public float Right { get; }
    public float Bottom { get; }
    public float Left { get; }

    [JsonConstructor]
    public Rectangle(float Left, float Top, float Right, float Bottom)
    {
        this.Left = Left;
        this.Top = Top;
        this.Right = Right;
        this.Bottom = Bottom;
        Width = MathF.Abs(this.Left - this.Right);
        Height = MathF.Abs(this.Top - this.Bottom);
    }

    public Rectangle(Vector2 Position, Size Size)
    {
        Left = Position.X;
        Top = Position.Y;
        Right = Left + Size.Width;
        Bottom = Top + Size.Height;
        Width = Size.Width;
        Height = Size.Height;
    }

    public bool Overlap(Rectangle R)
        => Left < R.Right && Right > R.Left && Top < R.Bottom && Bottom > R.Top;

    public bool Intersects(Rectangle R)
    {
        var t_ThisX = Left;
        var t_ThisY = Top;
        var t_ThisW = Width;
        var t_ThisH = Height;
        var t_RectX = R.Left;
        var t_RectY = R.Top;
        var t_RectW = R.Width;
        var t_RectH = R.Height;
        return t_RectX < t_ThisX + t_ThisW && t_ThisX < t_RectX + t_RectW && t_RectY < t_ThisY + t_ThisH && t_ThisY < t_RectY + t_RectH;
    }

    public Rectangle Inflate(float Horizontal, float Vertical)
        => new(Left - Horizontal, Top - Vertical, Right + Horizontal, Bottom + Vertical);

    public Rectangle Union(Rectangle R)
    {
        var t_X1 = MathF.Min(Left, R.Left);
        var t_X2 = MathF.Max(Left + Width, R.Left + R.Width);
        var t_Y1 = MathF.Min(Top, R.Top);
        var t_Y2 = MathF.Max(Top + Height, R.Top + R.Height);
        return new(t_X1, t_Y1, t_X2, t_Y2);
    }

    public bool ContainsPoint(Vector2 Point) => ContainsPoint(Point.X, Point.Y);

    public bool ContainsPoint(float X, float Y)
        => X >= Left && X <= Right && Y >= Top && Y <= Bottom;

    public IEnumerable<Vector2> GetIntersectionsWithLine(Line Line)
    {
        var t_Borders = new[] {
            new Line(NorthWest, NorthEast),
            new Line(NorthEast, SouthEast),
            new Line(SouthWest, SouthEast),
            new Line(NorthWest, SouthWest)
        };

        for (var t_I = 0; t_I < t_Borders.Length; t_I++)
        {
            var t_IntersectionPt = t_Borders[t_I].GetIntersection(Line);
            if (t_IntersectionPt != null)
                yield return t_IntersectionPt.Value;
        }
    }

    public Vector2? GetPointAtAngle(float A)
    {
        var t_Vx = MathF.Cos(A * MathF.PI / 180);
        var t_Vy = MathF.Sin(A * MathF.PI / 180);
        var t_Px = Left + Width / 2;
        var t_Py = Top + Height / 2;
        float? t_T1 = (Left - t_Px) / t_Vx; // left
        float? t_T2 = (Right - t_Px) / t_Vx; // right
        float? t_T3 = (Top - t_Py) / t_Vy; // top
        float? t_T4 = (Bottom - t_Py) / t_Vy; // bottom
        var t_T = (new[] { t_T1, t_T2, t_T3, t_T4 }).Where(N => N.HasValue && float.IsFinite(N.Value) && N.Value > 0).DefaultIfEmpty(null).Min();
        if (t_T == null) return null;

        var t_X = t_Px + t_T.Value * t_Vx;
        var t_Y = t_Py + t_T.Value * t_Vy;
        return new Vector2(t_X, t_Y);
    }

    public Vector2 Center => new(Left + Width / 2, Top + Height / 2);
    public Vector2 NorthEast => new(Right, Top);
    public Vector2 SouthEast => new(Right, Bottom);
    public Vector2 SouthWest => new(Left, Bottom);
    public Vector2 NorthWest => new(Left, Top);
    public Vector2 East => new(Right, Top + Height / 2);
    public Vector2 North => new(Left + Width / 2, Top);
    public Vector2 South => new(Left + Width / 2, Bottom);
    public Vector2 West => new(Left, Top + Height / 2);

    public bool Equals(Rectangle? Other)
    {
        return Other != null && Left == Other.Left && Right == Other.Right && Top == Other.Top &&
            Bottom == Other.Bottom && Width == Other.Width && Height == Other.Height;
    }

    public override string ToString()
                => $"Rectangle(width={Width}, height={Height}, top={Top}, right={Right}, bottom={Bottom}, left={Left})";
}
