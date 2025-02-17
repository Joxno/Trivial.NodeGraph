using Trivial.Domain.Geometry;
using Trivial.Domain.Models.Base;
using System;
using System.Numerics;

namespace Trivial.Domain.PathGenerators;

public abstract class PathGenerator
{
    public abstract PathGeneratorResult GetResult(Diagram diagram, BaseLinkModel link, Vector2[] route, Vector2 source, Vector2 target);

    protected static float AdjustRouteForSourceMarker(Vector2[] route, float markerWidth)
    {            
        var angleInRadians = MathF.Atan2(route[1].Y - route[0].Y, route[1].X - route[0].X) + MathF.PI;
        var xChange = markerWidth * MathF.Cos(angleInRadians);
        var yChange = markerWidth * MathF.Sin(angleInRadians);
        route[0] = new Vector2(route[0].X - xChange, route[0].Y - yChange);
        return angleInRadians * 180 / MathF.PI;
    }

    protected static float AdjustRouteForTargetMarker(Vector2[] route, float markerWidth)
    {
        var angleInRadians = MathF.Atan2(route[^1].Y - route[^2].Y, route[^1].X - route[^2].X);
        var xChange = markerWidth * MathF.Cos(angleInRadians);
        var yChange = markerWidth * MathF.Sin(angleInRadians);
        route[^1] = new Vector2(route[^1].X - xChange, route[^1].Y - yChange);
        return angleInRadians * 180 / MathF.PI;
    }

    protected static Vector2[] ConcatRouteAndSourceAndTarget(Vector2[] route, Vector2 source, Vector2 target)
    {
        var result = new Vector2[route.Length + 2];
        result[0] = source;
        route.CopyTo(result, 1);
        result[^1] = target;
        return result;
    }
}
