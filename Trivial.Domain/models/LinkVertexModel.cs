using Trivial.Domain.Geometry;
using Trivial.Domain.Models.Base;

namespace Trivial.Domain.Models;

public class LinkVertexModel : MovableModel
{
    public LinkVertexModel(BaseLinkModel parent, Point? position = null) : base(position)
    {
        Parent = parent;
    }

    public BaseLinkModel Parent { get; }

    public override void SetPosition(double x, double y)
    {
        base.SetPosition(x, y);
        Refresh();
        Parent.Refresh();
    }
}
