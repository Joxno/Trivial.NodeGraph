using System.Collections.Generic;
using System.Numerics;

namespace Trivial.Domain.Geometry;

public interface IShape
{
    public IEnumerable<Vector2> GetIntersectionsWithLine(Line line);
    public Vector2? GetPointAtAngle(float a);
}
