using System.Numerics;
using Trivial.Domain.Geometry;
using Trivial.Domain.Models.Base;
using Trivial.Domain.Positions;

namespace Trivial.Domain.Anchors;

public class LinkAnchor : Anchor
{
    private readonly LinkPathPositionProvider m_PositionProvider;

    public LinkAnchor(BaseLinkModel Link, float Distance, float OffsetX = 0, float OffsetY = 0) : base(Link)
    {
        m_PositionProvider = new LinkPathPositionProvider(Distance, OffsetX, OffsetY);
        this.Link = Link;
    }

    public BaseLinkModel Link { get; }

    public override Vector2? GetPosition(BaseLinkModel Link, Vector2[] Route) => m_PositionProvider.GetPosition(this.Link);

    public override Vector2? GetPlainPosition() => m_PositionProvider.GetPosition(Link);
}