using Trivial.Domain.Geometry;
using Trivial.Domain.Models;

namespace Trivial.Graph.Models;

public class SvgNodeModel : NodeModel
{
    public SvgNodeModel(Point? position = null) : base(position)
    {
    }

    public SvgNodeModel(string id, Point? position = null) : base(id, position)
    {
    }
}