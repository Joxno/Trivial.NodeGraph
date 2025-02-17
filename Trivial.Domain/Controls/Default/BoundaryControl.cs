using Trivial.Domain.Geometry;
using Trivial.Domain.Models.Base;

namespace Trivial.Domain.Controls.Default;

public class BoundaryControl : Control
{
    public Rectangle Bounds { get; private set; } = Rectangle.Zero;
    
    public override Point? GetPosition(Model model)
    {
        if (model is not IHasBounds hb)
            return null;

        var bounds = hb.GetBounds();
        if (bounds == null)
            return null;

        Bounds = bounds.Inflate(10, 10);
        return Bounds.NorthWest;
    }
}