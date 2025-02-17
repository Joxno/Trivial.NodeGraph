using Trivial.Domain.Geometry;

namespace Trivial.Domain.Models.Base;

public interface IHasShape
{
    public IShape GetShape();
}