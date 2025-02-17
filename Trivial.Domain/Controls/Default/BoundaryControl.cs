using System.Numerics;
using Trivial.Domain.Geometry;
using Trivial.Domain.Models.Base;

namespace Trivial.Domain.Controls.Default;

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