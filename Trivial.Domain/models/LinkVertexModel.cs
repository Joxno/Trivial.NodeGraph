using System.Numerics;
using Trivial.Domain.Geometry;
using Trivial.Domain.Models.Base;

namespace Trivial.Domain.Models;

public class LinkVertexModel : MovableModel
{
    public LinkVertexModel(BaseLinkModel parent, Vector2? position = null) : base(position)
    {
        Parent = parent;
    }

    public BaseLinkModel Parent { get; }

    public override void SetPosition(float x, float y)
    {
        base.SetPosition(x, y);
        Refresh();
        Parent.Refresh();
    }
}
