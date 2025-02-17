using Trivial.Domain.Geometry;
using Trivial.Domain.Models.Base;
using System.Linq;
using System.Numerics;

namespace Trivial.Domain.Routers;

public class NormalRouter : Router
{
    public override Vector2[] GetRoute(Diagram diagram, BaseLinkModel link)
    {
        return link.Vertices.Select(v => v.Position).ToArray();
    }
}
