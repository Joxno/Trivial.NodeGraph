using System.Collections.Generic;

namespace Trivial.Domain.Geometry;

public interface IShape
{
    public IEnumerable<Point> GetIntersectionsWithLine(Line line);
    public Point? GetPointAtAngle(double a);
}
