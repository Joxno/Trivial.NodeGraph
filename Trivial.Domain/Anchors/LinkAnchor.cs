using Trivial.Domain.Geometry;
using Trivial.Domain.Models.Base;
using Trivial.Domain.Positions;

namespace Trivial.Domain.Anchors;

public class LinkAnchor : Anchor
{
    private readonly LinkPathPositionProvider _positionProvider;

    public LinkAnchor(BaseLinkModel link, double distance, double offsetX = 0, double offsetY = 0) : base(link)
    {
        _positionProvider = new LinkPathPositionProvider(distance, offsetX, offsetY);
        Link = link;
    }

    public BaseLinkModel Link { get; }

    public override Point? GetPosition(BaseLinkModel link, Point[] route) => _positionProvider.GetPosition(Link);

    public override Point? GetPlainPosition() => _positionProvider.GetPosition(Link);
}