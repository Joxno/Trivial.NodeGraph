using Trivial.Domain.Geometry;

namespace Trivial.Domain.Models.Base;

public interface IHasBounds
{
    public Rectangle? GetBounds();
}