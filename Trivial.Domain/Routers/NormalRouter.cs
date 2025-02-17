using Trivial.Domain.Geometry;
using Trivial.Domain.Models.Base;
using System.Linq;

namespace Trivial.Domain.Routers;

public class NormalRouter : Router
{
    public override Point[] GetRoute(Diagram diagram, BaseLinkModel link)
    {
        return link.Vertices.Select(v => v.Position).ToArray();
    }
}
