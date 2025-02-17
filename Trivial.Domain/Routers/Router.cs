using System.Numerics;
using Trivial.Graph.Domain.Geometry;
using Trivial.Graph.Domain.Models;
using Trivial.Graph.Domain.Models.Base;

namespace Trivial.Graph.Domain.Routers;

public abstract class Router
{
    public abstract Vector2[] GetRoute(Diagram Diagram, BaseLinkModel Link);

    protected static Vector2 GetPortPositionBasedOnAlignment(PortModel Port)
    {
        var t_Pt = Port.Position;
        switch (Port.Alignment)
        {
            case PortAlignment.Top:
                return new Vector2(t_Pt.X + Port.Size.Width / 2, t_Pt.Y);
            case PortAlignment.TopRight:
                return new Vector2(t_Pt.X + Port.Size.Width, t_Pt.Y);
            case PortAlignment.Right:
                return new Vector2(t_Pt.X + Port.Size.Width, t_Pt.Y + Port.Size.Height / 2);
            case PortAlignment.BottomRight:
                return new Vector2(t_Pt.X + Port.Size.Width, t_Pt.Y + Port.Size.Height);
            case PortAlignment.Bottom:
                return new Vector2(t_Pt.X + Port.Size.Width / 2, t_Pt.Y + Port.Size.Height);
            case PortAlignment.BottomLeft:
                return new Vector2(t_Pt.X, t_Pt.Y + Port.Size.Height);
            case PortAlignment.Left:
                return new Vector2(t_Pt.X, t_Pt.Y + Port.Size.Height / 2);
            default:
                return t_Pt;
        }
    }
}
