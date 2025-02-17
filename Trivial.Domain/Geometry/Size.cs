namespace Trivial.Graph.Domain.Geometry;

public record struct Size(float Width, float Height);

public static class SizeExtensions
{
    public static Size Zero(this Size Size) => new(0, 0);
    public static bool IsVisibleSize(this Size Size) => !(Size.Width <= 0.0001f || Size.Height <= 0.0001f);
}
