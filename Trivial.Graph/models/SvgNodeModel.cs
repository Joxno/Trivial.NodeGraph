using System.Numerics;
using Trivial.Domain.Geometry;
using Trivial.Domain.Models;

namespace Trivial.Graph.Models;

public class SvgNodeModel : NodeModel
{
    public SvgNodeModel(Vector2? Position = null) : base(Position)
    {
    }

    public SvgNodeModel(string Id, Vector2? Position = null) : base(Id, Position)
    {
    }
}