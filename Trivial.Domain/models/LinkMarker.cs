using System;

namespace Trivial.Domain.Models;

public class LinkMarker
{
    public static LinkMarker Arrow { get; } = new LinkMarker("M 0 -5 10 0 0 5 z", 10);
    public static LinkMarker Circle { get; } = new LinkMarker("M 0, 0 a 5,5 0 1,0 10,0 a 5,5 0 1,0 -10,0", 10);
    public static LinkMarker Square { get; } = new LinkMarker("M 0 -5 10 -5 10 5 0 5 z", 10);

    public LinkMarker(string Path, float Width)
    {
        this.Path = Path;
        this.Width = Width;
    }

    public string Path { get; }
    public float Width { get; }

    public static LinkMarker NewArrow(float Width, float Height)
        => new LinkMarker(FormattableString.Invariant($"M 0 -{Height / 2} {Width} 0 0 {Height / 2}"), Width);

    public static LinkMarker NewCircle(float R)
        => new LinkMarker(FormattableString.Invariant($"M 0, 0 a {R},{R} 0 1,0 {R * 2},0 a {R},{R} 0 1,0 -{R * 2},0"), R * 2);

    public static LinkMarker NewRectangle(float Width, float Height)
        => new LinkMarker(FormattableString.Invariant($"M 0 -{Height / 2} {Width} -{Height / 2} {Width} {Height / 2} 0 {Height / 2} z"), Width);

    public static LinkMarker NewSquare(float Size) => NewRectangle(Size, Size);
}
