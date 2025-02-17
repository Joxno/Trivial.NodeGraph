using Trivial.Graph.Domain.Geometry;
using Trivial.Graph.Domain.Models.Base;
using SvgPathProperties;
using System;
using System.Numerics;
using Trivial.Graph.Domain.Extensions;

namespace Trivial.Graph.Domain.PathGenerators;

public class StraightPathGenerator : PathGenerator
{
    private readonly float m_Radius;

    public StraightPathGenerator(float Radius = 0)
    {
        m_Radius = Radius;
    }

    public override PathGeneratorResult GetResult(Diagram Diagram, BaseLinkModel Link, Vector2[] Route, Vector2 Source, Vector2 Target)
    {
        Route = ConcatRouteAndSourceAndTarget(Route, Source, Target);

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

        var t_Paths = Link.Vertices.Count > 0 ? new SvgPath[Route.Length - 1] : null;
        var t_FullPath = new SvgPath().AddMoveTo(Route[0].X, Route[0].Y);
        float? t_SecondDist = null;
        var t_LastPt = Route[0];

        for (var t_I = 0; t_I < Route.Length - 1; t_I++)
        {
            if (m_Radius > 0 && t_I > 0)
            {
                var t_Previous = Route[t_I - 1];
                var t_Current = Route[t_I];
                var t_Next = Route[t_I + 1];

                float? t_FirstDist = t_SecondDist ?? (t_Current.DistanceTo(t_Previous) / 2);
                t_SecondDist = t_Current.DistanceTo(t_Next) / 2;

                var t_P1 = -MathF.Min(m_Radius, t_FirstDist.Value);
                var t_P2 = -MathF.Min(m_Radius, t_SecondDist.Value);

                var t_Fp = t_Current.Lerp(t_Previous, t_P1);
                var t_Sp = t_Current.Lerp(t_Next, t_P2);

                t_FullPath.AddLineTo(t_Fp.X, t_Fp.Y).AddQuadraticBezierCurve(t_Current.X, t_Current.Y, t_Sp.X, t_Sp.Y);

                if (t_Paths != null)
                {
                    t_Paths[t_I - 1] = new SvgPath().AddMoveTo(t_LastPt.X, t_LastPt.Y).AddLineTo(t_Fp.X, t_Fp.Y).AddQuadraticBezierCurve(t_Current.X, t_Current.Y, t_Sp.X, t_Sp.Y);
                }

                t_LastPt = t_Sp;

                if (t_I == Route.Length - 2)
                {
                    t_FullPath.AddLineTo(Route[^1].X, Route[^1].Y);

                    if (t_Paths != null)
                    {
                        t_Paths[t_I] = new SvgPath().AddMoveTo(t_LastPt.X, t_LastPt.Y).AddLineTo(Route[^1].X, Route[^1].Y);
                    }
                }
            }
            else if (m_Radius == 0 || Route.Length == 2)
            {
                t_FullPath.AddLineTo(Route[t_I + 1].X, Route[t_I + 1].Y);

                if (t_Paths != null)
                {
                    t_Paths[t_I] = new SvgPath().AddMoveTo(Route[t_I].X, Route[t_I].Y).AddLineTo(Route[t_I + 1].X, Route[t_I + 1].Y);
                }
            }
        }

        return new PathGeneratorResult(t_FullPath, t_Paths ?? Array.Empty<SvgPath>(), t_SourceAngle, Route[0], t_TargetAngle, Route[^1]);
    }
}
