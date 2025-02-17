using System.Numerics;
using Trivial.Graph.Domain.Geometry;
using Trivial.Graph.Domain.Models.Base;

namespace Trivial.Graph.Domain.Controls.Default;

public class BoundaryControl : Control
{
    public Rectangle Bounds { get; private set; } = Rectangle.Zero;
    
    public override Vector2? GetPosition(Model Model)
    {
        if (Model is not IHasBounds t_Hb)
            return null;

        var t_Bounds = t_Hb.GetBounds();
        if (t_Bounds == null)
            return null;

        Bounds = t_Bounds.Inflate(10, 10);
        return Bounds.NorthWest;
    }
}