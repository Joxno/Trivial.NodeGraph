using System.Numerics;
using Trivial.Domain.Geometry;
using Trivial.Domain.Models;
using Trivial.Domain.Models.Base;

namespace Trivial.Domain.Routers;

public abstract class Router
{
    public abstract Vector2[] GetRoute(Diagram diagram, BaseLinkModel link);

    protected static Vector2 GetPortPositionBasedOnAlignment(PortModel port)
    {
        var pt = port.Position;
        switch (port.Alignment)
        {
            case PortAlignment.Top:
                return new Vector2(pt.X + port.Size.Width / 2, pt.Y);
            case PortAlignment.TopRight:
                return new Vector2(pt.X + port.Size.Width, pt.Y);
            case PortAlignment.Right:
                return new Vector2(pt.X + port.Size.Width, pt.Y + port.Size.Height / 2);
            case PortAlignment.BottomRight:
                return new Vector2(pt.X + port.Size.Width, pt.Y + port.Size.Height);
            case PortAlignment.Bottom:
                return new Vector2(pt.X + port.Size.Width / 2, pt.Y + port.Size.Height);
            case PortAlignment.BottomLeft:
                return new Vector2(pt.X, pt.Y + port.Size.Height);
            case PortAlignment.Left:
                return new Vector2(pt.X, pt.Y + port.Size.Height / 2);
            default:
                return pt;
        }
    }
}
