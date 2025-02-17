using Trivial.Graph.Domain.Geometry;

namespace Trivial.Graph.Domain.Models.Base;

public interface IHasBounds
{
    public Rectangle? GetBounds();
}