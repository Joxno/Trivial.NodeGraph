namespace Trivial.Domain.Geometry;

public record Size
{
    public static Size Zero { get; } = new(0, 0);

    public Size(float Width, float Height)
    {
        this.Width = Width;
        this.Height = Height;
    }

    public float Width { get; init; }
    public float Height { get; init; }
}
