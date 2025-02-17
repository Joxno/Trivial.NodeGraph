using System;
using System.Linq;
using System.Numerics;
using Trivial.Graph.Domain.Geometry;
using Trivial.Graph.Domain.Models;
using Trivial.Graph.Domain.Models.Base;
using Trivial.Graph.Domain.Positions;

namespace Trivial.Graph.Domain.Anchors;

public sealed class DynamicAnchor : Anchor
{
    public DynamicAnchor(NodeModel Model, IPositionProvider[] Providers) : base(Model)
    {
        if (Providers.Length == 0)
            throw new InvalidOperationException("No providers provided");

        Node = Model;
        this.Providers = Providers;
    }

    public NodeModel Node { get; }
    public IPositionProvider[] Providers { get; }

    public override Vector2? GetPosition(BaseLinkModel Link, Vector2[] Route)
    {
        if (!Node.Size.IsVisibleSize()) return null;

        var t_IsTarget = Link.Target == this;
        var t_Pt = Route.Length > 0 ? Route[t_IsTarget ? ^1 : 0] : GetOtherPosition(Link, t_IsTarget);
        var t_Positions = Providers.Select(E => E.GetPosition(Node)).Where(P => P.HasValue).Select(P => P.Value);
        return t_Pt is null ? null : GetClosestPointTo(t_Positions, t_Pt.Value);
    }

    public override Vector2? GetPlainPosition() => Node.GetBounds()?.Center ?? null;
}