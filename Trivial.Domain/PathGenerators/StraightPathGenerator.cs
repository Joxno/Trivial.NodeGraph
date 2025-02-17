using Trivial.Domain.Geometry;
using Trivial.Domain.Models.Base;
using SvgPathProperties;
using System;
using System.Numerics;
using Trivial.Domain.Extensions;

namespace Trivial.Domain.PathGenerators;

public class StraightPathGenerator : PathGenerator
{
    private readonly float _radius;

    public StraightPathGenerator(float radius = 0)
    {
        _radius = radius;
    }

    public override PathGeneratorResult GetResult(Diagram diagram, BaseLinkModel link, Vector2[] route, Vector2 source, Vector2 target)
    {
        route = ConcatRouteAndSourceAndTarget(route, source, target);

        float? sourceAngle = null;
        float? targetAngle = null;

        if (link.SourceMarker != null)
        {
            sourceAngle = AdjustRouteForSourceMarker(route, link.SourceMarker.Width);
        }

        if (link.TargetMarker != null)
        {
            targetAngle = AdjustRouteForTargetMarker(route, link.TargetMarker.Width);
        }

        var paths = link.Vertices.Count > 0 ? new SvgPath[route.Length - 1] : null;
        var fullPath = new SvgPath().AddMoveTo(route[0].X, route[0].Y);
        float? secondDist = null;
        var lastPt = route[0];

        for (var i = 0; i < route.Length - 1; i++)
        {
            if (_radius > 0 && i > 0)
            {
                var previous = route[i - 1];
                var current = route[i];
                var next = route[i + 1];

                float? firstDist = secondDist ?? (current.DistanceTo(previous) / 2);
                secondDist = current.DistanceTo(next) / 2;

                var p1 = -MathF.Min(_radius, firstDist.Value);
                var p2 = -MathF.Min(_radius, secondDist.Value);

                var fp = current.Lerp(previous, p1);
                var sp = current.Lerp(next, p2);

                fullPath.AddLineTo(fp.X, fp.Y).AddQuadraticBezierCurve(current.X, current.Y, sp.X, sp.Y);

                if (paths != null)
                {
                    paths[i - 1] = new SvgPath().AddMoveTo(lastPt.X, lastPt.Y).AddLineTo(fp.X, fp.Y).AddQuadraticBezierCurve(current.X, current.Y, sp.X, sp.Y);
                }

                lastPt = sp;

                if (i == route.Length - 2)
                {
                    fullPath.AddLineTo(route[^1].X, route[^1].Y);

                    if (paths != null)
                    {
                        paths[i] = new SvgPath().AddMoveTo(lastPt.X, lastPt.Y).AddLineTo(route[^1].X, route[^1].Y);
                    }
                }
            }
            else if (_radius == 0 || route.Length == 2)
            {
                fullPath.AddLineTo(route[i + 1].X, route[i + 1].Y);

                if (paths != null)
                {
                    paths[i] = new SvgPath().AddMoveTo(route[i].X, route[i].Y).AddLineTo(route[i + 1].X, route[i + 1].Y);
                }
            }
        }

        return new PathGeneratorResult(fullPath, paths ?? Array.Empty<SvgPath>(), sourceAngle, route[0], targetAngle, route[^1]);
    }
}
