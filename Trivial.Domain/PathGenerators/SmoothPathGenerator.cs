using Trivial.Graph.Domain.Anchors;
using Trivial.Graph.Domain.Geometry;
using Trivial.Graph.Domain.Models;
using Trivial.Graph.Domain.Models.Base;
using SvgPathProperties;
using System;
using System.Numerics;

namespace Trivial.Graph.Domain.PathGenerators;

public class SmoothPathGenerator : PathGenerator
{
    private readonly float m_Margin;

    public SmoothPathGenerator(float Margin = 125)
    {
        m_Margin = Margin;
    }

    public override PathGeneratorResult GetResult(Diagram Diagram, BaseLinkModel Link, Vector2[] Route, Vector2 Source, Vector2 Target)
    {
        Route = ConcatRouteAndSourceAndTarget(Route, Source, Target);

        if (Route.Length > 2)
            return CurveThroughPoints(Route, Link);

        Route = GetRouteWithCurvePoints(Link, Route);
        float? t_SourceAngle = null;
        float? t_TargetAngle = null;

        if (Link.SourceMarker != null)
        {
            t_SourceAngle = AdjustRouteForSourceMarker(Route, Link.SourceMarker.Width);
        }

        if (Link.TargetMarker != null)
        {
            t_TargetAngle = AdjustRouteForTargetMarker(Route, Link.TargetMarker.Width);
        }

        var t_Path = new SvgPath()
            .AddMoveTo(Route[0].X, Route[0].Y)
            .AddCubicBezierCurve(Route[1].X, Route[1].Y, Route[2].X, Route[2].Y, Route[3].X, Route[3].Y);

        return new PathGeneratorResult(t_Path, Array.Empty<SvgPath>(), t_SourceAngle, Route[0], t_TargetAngle, Route[^1]);
    }

    private PathGeneratorResult CurveThroughPoints(Vector2[] Route, BaseLinkModel Link)
    {
        float? t_SourceAngle = null;
        float? t_TargetAngle = null;

        if (Link.SourceMarker != null)
        {
            t_SourceAngle = AdjustRouteForSourceMarker(Route, Link.SourceMarker.Width);
        }

        if (Link.TargetMarker != null)
        {
            t_TargetAngle = AdjustRouteForTargetMarker(Route, Link.TargetMarker.Width);
        }

        BezierSpline.GetCurveControlPoints(Route, out var t_FirstControlPoints, out var t_SecondControlPoints);
        var t_Paths = new SvgPath[t_FirstControlPoints.Length];
        var t_FullPath = new SvgPath().AddMoveTo(Route[0].X, Route[0].Y);

        for (var t_I = 0; t_I < t_FirstControlPoints.Length; t_I++)
        {
            var t_Cp1 = t_FirstControlPoints[t_I];
            var t_Cp2 = t_SecondControlPoints[t_I];
            t_FullPath.AddCubicBezierCurve(t_Cp1.X, t_Cp1.Y, t_Cp2.X, t_Cp2.Y, Route[t_I + 1].X, Route[t_I + 1].Y);
            t_Paths[t_I] = new SvgPath().AddMoveTo(Route[t_I].X, Route[t_I].Y).AddCubicBezierCurve(t_Cp1.X, t_Cp1.Y, t_Cp2.X, t_Cp2.Y, Route[t_I + 1].X, Route[t_I + 1].Y);
        }

        // Todo: adjust marker positions based on closest control points
        return new PathGeneratorResult(t_FullPath, t_Paths, t_SourceAngle, Route[0], t_TargetAngle, Route[^1]);
    }

    private Vector2[] GetRouteWithCurvePoints(BaseLinkModel Link, Vector2[] Route)
    {
        var t_CX = (Route[0].X + Route[1].X) / 2;
        var t_CY = (Route[0].Y + Route[1].Y) / 2;
        var t_CurvePointA = GetCurvePoint(Route, Link.Source, Route[0].X, Route[0].Y, t_CX, t_CY, First: true);
        var t_CurvePointB = GetCurvePoint(Route, Link.Target, Route[1].X, Route[1].Y, t_CX, t_CY, First: false);
        return new[] { Route[0], t_CurvePointA, t_CurvePointB, Route[1] };
    }

    private Vector2 GetCurvePoint(Vector2[] Route, Anchor Anchor, float PX, float PY, float CX, float CY, bool First)
    {
        if (Anchor is PositionAnchor)
            return new Vector2(CX, CY);

        if (Anchor is SinglePortAnchor t_Spa)
        {
            return GetCurvePoint(PX, PY, CX, CY, t_Spa.Port.Alignment);
        }
        else if (Anchor is ShapeIntersectionAnchor or DynamicAnchor or LinkAnchor)
        {
            if (MathF.Abs(Route[0].X - Route[1].X) >= MathF.Abs(Route[0].Y - Route[1].Y))
            {
                return First ? new Vector2(CX, Route[0].Y) : new Vector2(CX, Route[1].Y);
            }
            else
            {
                return First ? new Vector2(Route[0].X, CY) : new Vector2(Route[1].X, CY);
            }
        }
        else
        {
            throw new DiagramsException($"Unhandled Anchor type {Anchor.GetType().Name} when trying to find curve point");
        }
    }

    private Vector2 GetCurvePoint(float PX, float PY, float CX, float CY, PortAlignment? Alignment)
    {
        var t_Margin = MathF.Min(m_Margin, MathF.Pow(MathF.Pow(PX - CX, 2) + MathF.Pow(PY - CY, 2), .5f));
        return Alignment switch
        {
            PortAlignment.Top => new Vector2(PX, MathF.Min(PY - t_Margin, CY)),
            PortAlignment.Bottom => new Vector2(PX, MathF.Max(PY + t_Margin, CY)),
            PortAlignment.TopRight => new Vector2(MathF.Max(PX + t_Margin, CX), MathF.Min(PY - t_Margin, CY)),
            PortAlignment.BottomRight => new Vector2(MathF.Max(PX + t_Margin, CX), MathF.Max(PY + t_Margin, CY)),
            PortAlignment.Right => new Vector2(MathF.Max(PX + t_Margin, CX), PY),
            PortAlignment.Left => new Vector2(MathF.Min(PX - t_Margin, CX), PY),
            PortAlignment.BottomLeft => new Vector2(MathF.Min(PX - t_Margin, CX), MathF.Max(PY + t_Margin, CY)),
            PortAlignment.TopLeft => new Vector2(MathF.Min(PX - t_Margin, CX), MathF.Min(PY - t_Margin, CY)),
            _ => new Vector2(CX, CY),
        };
    }
}
