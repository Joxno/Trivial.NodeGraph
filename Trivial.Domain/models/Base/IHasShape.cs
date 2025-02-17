using Trivial.Graph.Domain.Geometry;

namespace Trivial.Graph.Domain.Models.Base;

public interface IHasShape
{
    public IShape GetShape();
}