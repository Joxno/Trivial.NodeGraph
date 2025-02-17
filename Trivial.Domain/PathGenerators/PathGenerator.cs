using Trivial.Domain.Geometry;
using Trivial.Domain.Models.Base;
using System;
using System.Numerics;

namespace Trivial.Domain.PathGenerators;

public abstract class PathGenerator
{
    public abstract PathGeneratorResult GetResult(Diagram Diagram, BaseLinkModel Link, Vector2[] Route, Vector2 Source, Vector2 Target);

    protected static float AdjustRouteForSourceMarker(Vector2[] Route, float MarkerWidth)
    {            
        var t_AngleInRadians = MathF.Atan2(Route[1].Y - Route[0].Y, Route[1].X - Route[0].X) + MathF.PI;
        var t_XChange = MarkerWidth * MathF.Cos(t_AngleInRadians);
        var t_YChange = MarkerWidth * MathF.Sin(t_AngleInRadians);
        Route[0] = new Vector2(Route[0].X - t_XChange, Route[0].Y - t_YChange);
        return t_AngleInRadians * 180 / MathF.PI;
    }

    protected static float AdjustRouteForTargetMarker(Vector2[] Route, float MarkerWidth)
    {
        var t_AngleInRadians = MathF.Atan2(Route[^1].Y - Route[^2].Y, Route[^1].X - Route[^2].X);
        var t_XChange = MarkerWidth * MathF.Cos(t_AngleInRadians);
        var t_YChange = MarkerWidth * MathF.Sin(t_AngleInRadians);
        Route[^1] = new Vector2(Route[^1].X - t_XChange, Route[^1].Y - t_YChange);
        return t_AngleInRadians * 180 / MathF.PI;
    }

    protected static Vector2[] ConcatRouteAndSourceAndTarget(Vector2[] Route, Vector2 Source, Vector2 Target)
    {
        var t_Result = new Vector2[Route.Length + 2];
        t_Result[0] = Source;
        Route.CopyTo(t_Result, 1);
        t_Result[^1] = Target;
        return t_Result;
    }
}
