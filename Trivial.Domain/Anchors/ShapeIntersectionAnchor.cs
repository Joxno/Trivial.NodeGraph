using System.Numerics;
using Trivial.Domain.Geometry;
using Trivial.Domain.Models;
using Trivial.Domain.Models.Base;

namespace Trivial.Domain.Anchors;

public sealed class ShapeIntersectionAnchor : Anchor
{
    public ShapeIntersectionAnchor(NodeModel model) : base(model)
    {
        Node = model;
    }

    public NodeModel Node { get; }

    public override Vector2? GetPosition(BaseLinkModel link, Vector2[] route)
    {
        if (Node.Size == null)
            return null;

        var isTarget = link.Target == this;
        var nodeCenter = Node.GetBounds()!.Center;
        Vector2? pt;
        if (route.Length > 0)
        {
            pt = route[isTarget ? ^1 : 0];
        }
        else
        {
            pt = GetOtherPosition(link, isTarget);
        }

        if (pt is null) return null;

        var line = new Line(pt.Value, nodeCenter);
        var intersections = Node.GetShape().GetIntersectionsWithLine(line);
        return GetClosestPointTo(intersections, pt.Value);
    }

    public override Vector2? GetPlainPosition() => Node.GetBounds()?.Center ?? null;
}
