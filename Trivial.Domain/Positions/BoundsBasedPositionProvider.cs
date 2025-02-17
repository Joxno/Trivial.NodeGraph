using System.Numerics;
using Trivial.Domain.Geometry;
using Trivial.Domain.Models.Base;

namespace Trivial.Domain.Positions;

public class BoundsBasedPositionProvider : IPositionProvider
{
    public BoundsBasedPositionProvider(float x, float y, float offsetX = 0, float offsetY = 0)
    {
        X = x;
        Y = y;
        OffsetX = offsetX;
        OffsetY = offsetY;
    }

    public float X { get; }
    public float Y { get; }
    public float OffsetX { get; }
    public float OffsetY { get; }

    public Vector2? GetPosition(Model model)
    {
        if (model is not IHasBounds ihb)
            throw new DiagramsException("BoundsBasedPositionProvider requires an IHasBounds model");
        
        var bounds = ihb.GetBounds();
        if (bounds == null)
            return null;
        
        return new Vector2(bounds.Left + X * bounds.Width + OffsetX, bounds.Top + Y * bounds.Height + OffsetY);
    }
}