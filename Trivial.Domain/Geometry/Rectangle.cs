using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Trivial.Domain.Geometry;

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
    public Rectangle(float left, float top, float right, float bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
        Width = MathF.Abs(Left - Right);
        Height = MathF.Abs(Top - Bottom);
    }

    public Rectangle(Vector2 position, Size size)
    {
        ArgumentNullException.ThrowIfNull(position, nameof(position));
        ArgumentNullException.ThrowIfNull(size, nameof(size));

        Left = position.X;
        Top = position.Y;
        Right = Left + size.Width;
        Bottom = Top + size.Height;
        Width = size.Width;
        Height = size.Height;
    }

    public bool Overlap(Rectangle r)
        => Left < r.Right && Right > r.Left && Top < r.Bottom && Bottom > r.Top;

    public bool Intersects(Rectangle r)
    {
        var thisX = Left;
        var thisY = Top;
        var thisW = Width;
        var thisH = Height;
        var rectX = r.Left;
        var rectY = r.Top;
        var rectW = r.Width;
        var rectH = r.Height;
        return rectX < thisX + thisW && thisX < rectX + rectW && rectY < thisY + thisH && thisY < rectY + rectH;
    }

    public Rectangle Inflate(float horizontal, float vertical)
        => new(Left - horizontal, Top - vertical, Right + horizontal, Bottom + vertical);

    public Rectangle Union(Rectangle r)
    {
        var x1 = MathF.Min(Left, r.Left);
        var x2 = MathF.Max(Left + Width, r.Left + r.Width);
        var y1 = MathF.Min(Top, r.Top);
        var y2 = MathF.Max(Top + Height, r.Top + r.Height);
        return new(x1, y1, x2, y2);
    }

    public bool ContainsPoint(Vector2 point) => ContainsPoint(point.X, point.Y);

    public bool ContainsPoint(float x, float y)
        => x >= Left && x <= Right && y >= Top && y <= Bottom;

    public IEnumerable<Vector2> GetIntersectionsWithLine(Line line)
    {
        var borders = new[] {
            new Line(NorthWest, NorthEast),
            new Line(NorthEast, SouthEast),
            new Line(SouthWest, SouthEast),
            new Line(NorthWest, SouthWest)
        };

        for (var i = 0; i < borders.Length; i++)
        {
            var intersectionPt = borders[i].GetIntersection(line);
            if (intersectionPt != null)
                yield return intersectionPt.Value;
        }
    }

    public Vector2? GetPointAtAngle(float a)
    {
        var vx = MathF.Cos(a * MathF.PI / 180);
        var vy = MathF.Sin(a * MathF.PI / 180);
        var px = Left + Width / 2;
        var py = Top + Height / 2;
        float? t1 = (Left - px) / vx; // left
        float? t2 = (Right - px) / vx; // right
        float? t3 = (Top - py) / vy; // top
        float? t4 = (Bottom - py) / vy; // bottom
        var t = (new[] { t1, t2, t3, t4 }).Where(n => n.HasValue && float.IsFinite(n.Value) && n.Value > 0).DefaultIfEmpty(null).Min();
        if (t == null) return null;

        var x = px + t.Value * vx;
        var y = py + t.Value * vy;
        return new Vector2(x, y);
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

    public bool Equals(Rectangle? other)
    {
        return other != null && Left == other.Left && Right == other.Right && Top == other.Top &&
            Bottom == other.Bottom && Width == other.Width && Height == other.Height;
    }

    public override string ToString()
                => $"Rectangle(width={Width}, height={Height}, top={Top}, right={Right}, bottom={Bottom}, left={Left})";
}
