using System.Numerics;
using Trivial.Domain.Geometry;
using Trivial.Domain.Models.Base;

namespace Trivial.Domain.Anchors;

public sealed class PositionAnchor : Anchor
{
    private Vector2 m_Position;

    public PositionAnchor(Vector2 Position) : base(null)
    {
        m_Position = Position;
    }

    public void SetPosition(Vector2 Position) => m_Position = Position;

    public override Vector2? GetPlainPosition() => m_Position;

    public override Vector2? GetPosition(BaseLinkModel Link, Vector2[] Route) => m_Position;
}
