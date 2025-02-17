using System.Numerics;
using Trivial.Graph.Domain.exceptions;
using Trivial.Graph.Domain.Geometry;
using Trivial.Graph.Domain.Models.Base;

namespace Trivial.Graph.Domain.Positions;

public class ShapeAnglePositionProvider : IPositionProvider
{
    public ShapeAnglePositionProvider(float Angle, float OffsetX = 0, float OffsetY = 0)
    {
        this.Angle = Angle;
        this.OffsetX = OffsetX;
        this.OffsetY = OffsetY;
    }

    public float Angle { get; }
    public float OffsetX { get; }
    public float OffsetY { get; }
    
    public Vector2? GetPosition(Model Model)
    {
        if (Model is not IHasShape t_Ihs)
            throw new DiagramsException("ShapeAnglePositionProvider requires an IHasShape model");
        
        var t_Shape = t_Ihs.GetShape();
        return t_Shape.GetPointAtAngle(Angle)! + new Vector2(OffsetX, OffsetY);
    }
}