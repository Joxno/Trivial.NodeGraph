using System.Numerics;
using Trivial.Domain.Geometry;
using Trivial.Domain.Models;
using Trivial.Domain.Models.Base;

namespace Trivial.Domain.Anchors;

public sealed class ShapeIntersectionAnchor : Anchor
{
    public ShapeIntersectionAnchor(NodeModel Model) : base(Model)
    {
        Node = Model;
    }

    public NodeModel Node { get; }

    public override Vector2? GetPosition(BaseLinkModel Link, Vector2[] Route)
    {
        if (!Node.Size.IsVisibleSize()) return null;

        var t_IsTarget = Link.Target == this;
        var t_NodeCenter = Node.GetBounds()!.Center;
        Vector2? t_Pt;
        if (Route.Length > 0)
        {
            t_Pt = Route[t_IsTarget ? ^1 : 0];
        }
        else
        {
            t_Pt = GetOtherPosition(Link, t_IsTarget);
        }

        if (t_Pt is null) return null;

        var t_Line = new Line(t_Pt.Value, t_NodeCenter);
        var t_Intersections = Node.GetShape().GetIntersectionsWithLine(t_Line);
        return GetClosestPointTo(t_Intersections, t_Pt.Value);
    }

    public override Vector2? GetPlainPosition() => Node.GetBounds()?.Center ?? null;
}
