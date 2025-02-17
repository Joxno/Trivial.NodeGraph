using System.Numerics;
using Trivial.Domain.Geometry;
using Trivial.Domain.Models.Base;

namespace Trivial.Domain.Models;

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
