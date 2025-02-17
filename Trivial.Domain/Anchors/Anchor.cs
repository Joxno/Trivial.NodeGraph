using Trivial.Domain.Geometry;
using Trivial.Domain.Models.Base;
using System;
using System.Collections.Generic;
using System.Numerics;
using Trivial.Domain.Extensions;

namespace Trivial.Domain.Anchors;

public abstract class Anchor
{
    protected Anchor(ILinkable? Model = null)
    {
        this.Model = Model;
    }

    public ILinkable? Model { get; }

    public abstract Vector2? GetPosition(BaseLinkModel Link, Vector2[] Route);

    public abstract Vector2? GetPlainPosition();

    public Vector2? GetPosition(BaseLinkModel Link) => GetPosition(Link, Array.Empty<Vector2>());

    protected static Vector2? GetOtherPosition(BaseLinkModel Link, bool IsTarget)
    {
        var t_Anchor = IsTarget ? Link.Source : Link.Target!;
        return t_Anchor.GetPlainPosition();
    }

    protected static Vector2? GetClosestPointTo(IEnumerable<Vector2> Points, Vector2 Point)
    {
        var t_MinDist = float.MaxValue;
        Vector2? t_MinPoint = null;

        foreach (var t_Pt in Points)
        {
            if (t_Pt == null)
                continue;
            
            var t_Dist = t_Pt.DistanceTo(Point);
            if (t_Dist < t_MinDist)
            {
                t_MinDist = t_Dist;
                t_MinPoint = t_Pt;
            }
        }

        return t_MinPoint;
    }
}
