namespace Trivial.Domain.Geometry;

public record struct Size(float Width, float Height);

public static class SizeExtensions
{
    public static Size Zero(this Size Size) => new(0, 0);
}
