using System.Numerics;
using Trivial.Domain.Geometry;
using Trivial.Domain.Models.Base;
using Trivial.Domain.Positions;

namespace Trivial.Domain.Anchors;

public class LinkAnchor : Anchor
{
    private readonly LinkPathPositionProvider _positionProvider;

    public LinkAnchor(BaseLinkModel link, float distance, float offsetX = 0, float offsetY = 0) : base(link)
    {
        _positionProvider = new LinkPathPositionProvider(distance, offsetX, offsetY);
        Link = link;
    }

    public BaseLinkModel Link { get; }

    public override Vector2? GetPosition(BaseLinkModel link, Vector2[] route) => _positionProvider.GetPosition(Link);

    public override Vector2? GetPlainPosition() => _positionProvider.GetPosition(Link);
}