using System.Numerics;
using Trivial.Domain.Geometry;
using Trivial.Domain.Models.Base;

namespace Trivial.Domain.Anchors;

public sealed class PositionAnchor : Anchor
{
    private Vector2 _position;

    public PositionAnchor(Vector2 position) : base(null)
    {
        _position = position;
    }

    public void SetPosition(Vector2 position) => _position = position;

    public override Vector2? GetPlainPosition() => _position;

    public override Vector2? GetPosition(BaseLinkModel link, Vector2[] route) => _position;
}
