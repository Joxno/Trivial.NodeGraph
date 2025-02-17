using Trivial.Graph.Domain.Geometry;
using Trivial.Graph.Domain.Models.Base;
using System.Linq;
using System.Numerics;

namespace Trivial.Graph.Domain.Routers;

public class NormalRouter : Router
{
    public override Vector2[] GetRoute(Diagram Diagram, BaseLinkModel Link)
    {
        return Link.Vertices.Select(V => V.Position).ToArray();
    }
}
