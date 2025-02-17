using Trivial.Domain.Anchors;
using Trivial.Domain.Geometry;
using Trivial.Domain.Models;
using Trivial.Domain.Models.Base;
using SvgPathProperties;
using System;
using System.Numerics;

namespace Trivial.Domain.PathGenerators;

public class SmoothPathGenerator : PathGenerator
{
    private readonly float _margin;

    public SmoothPathGenerator(float margin = 125)
    {
        _margin = margin;
    }

    public override PathGeneratorResult GetResult(Diagram diagram, BaseLinkModel link, Vector2[] route, Vector2 source, Vector2 target)
    {
        route = ConcatRouteAndSourceAndTarget(route, source, target);

        if (route.Length > 2)
            return CurveThroughPoints(route, link);

        route = GetRouteWithCurvePoints(link, route);
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

        var path = new SvgPath()
            .AddMoveTo(route[0].X, route[0].Y)
            .AddCubicBezierCurve(route[1].X, route[1].Y, route[2].X, route[2].Y, route[3].X, route[3].Y);

        return new PathGeneratorResult(path, Array.Empty<SvgPath>(), sourceAngle, route[0], targetAngle, route[^1]);
    }

    private PathGeneratorResult CurveThroughPoints(Vector2[] route, BaseLinkModel link)
    {
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

        BezierSpline.GetCurveControlPoints(route, out var firstControlPoints, out var secondControlPoints);
        var paths = new SvgPath[firstControlPoints.Length];
        var fullPath = new SvgPath().AddMoveTo(route[0].X, route[0].Y);

        for (var i = 0; i < firstControlPoints.Length; i++)
        {
            var cp1 = firstControlPoints[i];
            var cp2 = secondControlPoints[i];
            fullPath.AddCubicBezierCurve(cp1.X, cp1.Y, cp2.X, cp2.Y, route[i + 1].X, route[i + 1].Y);
            paths[i] = new SvgPath().AddMoveTo(route[i].X, route[i].Y).AddCubicBezierCurve(cp1.X, cp1.Y, cp2.X, cp2.Y, route[i + 1].X, route[i + 1].Y);
        }

        // Todo: adjust marker positions based on closest control points
        return new PathGeneratorResult(fullPath, paths, sourceAngle, route[0], targetAngle, route[^1]);
    }

    private Vector2[] GetRouteWithCurvePoints(BaseLinkModel link, Vector2[] route)
    {
        var cX = (route[0].X + route[1].X) / 2;
        var cY = (route[0].Y + route[1].Y) / 2;
        var curvePointA = GetCurvePoint(route, link.Source, route[0].X, route[0].Y, cX, cY, first: true);
        var curvePointB = GetCurvePoint(route, link.Target, route[1].X, route[1].Y, cX, cY, first: false);
        return new[] { route[0], curvePointA, curvePointB, route[1] };
    }

    private Vector2 GetCurvePoint(Vector2[] route, Anchor anchor, float pX, float pY, float cX, float cY, bool first)
    {
        if (anchor is PositionAnchor)
            return new Vector2(cX, cY);

        if (anchor is SinglePortAnchor spa)
        {
            return GetCurvePoint(pX, pY, cX, cY, spa.Port.Alignment);
        }
        else if (anchor is ShapeIntersectionAnchor or DynamicAnchor or LinkAnchor)
        {
            if (MathF.Abs(route[0].X - route[1].X) >= MathF.Abs(route[0].Y - route[1].Y))
            {
                return first ? new Vector2(cX, route[0].Y) : new Vector2(cX, route[1].Y);
            }
            else
            {
                return first ? new Vector2(route[0].X, cY) : new Vector2(route[1].X, cY);
            }
        }
        else
        {
            throw new DiagramsException($"Unhandled Anchor type {anchor.GetType().Name} when trying to find curve point");
        }
    }

    private Vector2 GetCurvePoint(float pX, float pY, float cX, float cY, PortAlignment? alignment)
    {
        var margin = MathF.Min(_margin, MathF.Pow(MathF.Pow(pX - cX, 2) + MathF.Pow(pY - cY, 2), .5f));
        return alignment switch
        {
            PortAlignment.Top => new Vector2(pX, MathF.Min(pY - margin, cY)),
            PortAlignment.Bottom => new Vector2(pX, MathF.Max(pY + margin, cY)),
            PortAlignment.TopRight => new Vector2(MathF.Max(pX + margin, cX), MathF.Min(pY - margin, cY)),
            PortAlignment.BottomRight => new Vector2(MathF.Max(pX + margin, cX), MathF.Max(pY + margin, cY)),
            PortAlignment.Right => new Vector2(MathF.Max(pX + margin, cX), pY),
            PortAlignment.Left => new Vector2(MathF.Min(pX - margin, cX), pY),
            PortAlignment.BottomLeft => new Vector2(MathF.Min(pX - margin, cX), MathF.Max(pY + margin, cY)),
            PortAlignment.TopLeft => new Vector2(MathF.Min(pX - margin, cX), MathF.Min(pY - margin, cY)),
            _ => new Vector2(cX, cY),
        };
    }
}
