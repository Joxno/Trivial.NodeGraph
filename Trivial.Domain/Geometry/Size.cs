namespace Trivial.Domain.Geometry;

public record Size
{
    public static Size Zero { get; } = new(0, 0);

    public Size(float width, float height)
    {
        Width = width;
        Height = height;
    }

    public float Width { get; init; }
    public float Height { get; init; }
}
