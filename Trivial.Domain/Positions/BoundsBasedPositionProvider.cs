using System.Numerics;
using Trivial.Graph.Domain.exceptions;
using Trivial.Graph.Domain.Geometry;
using Trivial.Graph.Domain.Models.Base;

namespace Trivial.Graph.Domain.Positions;

public class BoundsBasedPositionProvider : IPositionProvider
{
    public BoundsBasedPositionProvider(float X, float Y, float OffsetX = 0, float OffsetY = 0)
    {
        this.X = X;
        this.Y = Y;
        this.OffsetX = OffsetX;
        this.OffsetY = OffsetY;
    }

    public float X { get; }
    public float Y { get; }
    public float OffsetX { get; }
    public float OffsetY { get; }

    public Vector2? GetPosition(Model Model)
    {
        if (Model is not IHasBounds t_Ihb)
            throw new DiagramsException("BoundsBasedPositionProvider requires an IHasBounds model");
        
        var t_Bounds = t_Ihb.GetBounds();
        if (t_Bounds == null)
            return null;
        
        return new Vector2(t_Bounds.Left + X * t_Bounds.Width + OffsetX, t_Bounds.Top + Y * t_Bounds.Height + OffsetY);
    }
}