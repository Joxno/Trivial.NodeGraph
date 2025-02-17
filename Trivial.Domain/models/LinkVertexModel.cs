using System.Numerics;
using Trivial.Graph.Domain.Geometry;
using Trivial.Graph.Domain.Models.Base;

namespace Trivial.Graph.Domain.Models;

public class LinkVertexModel : MovableModel
{
    public LinkVertexModel(BaseLinkModel Parent, Vector2? Position = null) : base(Position)
    {
        this.Parent = Parent;
    }

    public BaseLinkModel Parent { get; }

    public override void SetPosition(float X, float Y)
    {
        base.SetPosition(X, Y);
        Refresh();
        Parent.Refresh();
    }
}
