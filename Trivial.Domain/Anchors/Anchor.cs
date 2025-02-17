using Trivial.Domain.Geometry;
using Trivial.Domain.Models.Base;
using System;
using System.Collections.Generic;
using System.Numerics;
using Trivial.Domain.Extensions;

namespace Trivial.Domain.Anchors;

public abstract class Anchor
{
    protected Anchor(ILinkable? model = null)
    {
        Model = model;
    }

    public ILinkable? Model { get; }

    public abstract Vector2? GetPosition(BaseLinkModel link, Vector2[] route);

    public abstract Vector2? GetPlainPosition();

    public Vector2? GetPosition(BaseLinkModel link) => GetPosition(link, Array.Empty<Vector2>());

    protected static Vector2? GetOtherPosition(BaseLinkModel link, bool isTarget)
    {
        var anchor = isTarget ? link.Source : link.Target!;
        return anchor.GetPlainPosition();
    }

    protected static Vector2? GetClosestPointTo(IEnumerable<Vector2> points, Vector2 point)
    {
        var minDist = float.MaxValue;
        Vector2? minPoint = null;

        foreach (var pt in points)
        {
            if (pt == null)
                continue;
            
            var dist = pt.DistanceTo(point);
            if (dist < minDist)
            {
                minDist = dist;
                minPoint = pt;
            }
        }

        return minPoint;
    }
}
