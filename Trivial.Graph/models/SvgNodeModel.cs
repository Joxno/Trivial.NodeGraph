using System.Numerics;
using Trivial.Domain.Geometry;
using Trivial.Domain.Models;

namespace Trivial.Graph.Models;

public class SvgNodeModel : NodeModel
{
    public SvgNodeModel(Vector2? position = null) : base(position)
    {
    }

    public SvgNodeModel(string id, Vector2? position = null) : base(id, position)
    {
    }
}