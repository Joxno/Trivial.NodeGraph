using System.Numerics;
using Trivial.Domain.Geometry;
using Trivial.Domain.Models.Base;

namespace Trivial.Domain.Positions;

public class ShapeAnglePositionProvider : IPositionProvider
{
    public ShapeAnglePositionProvider(float angle, float offsetX = 0, float offsetY = 0)
    {
        Angle = angle;
        OffsetX = offsetX;
        OffsetY = offsetY;
    }

    public float Angle { get; }
    public float OffsetX { get; }
    public float OffsetY { get; }
    
    public Vector2? GetPosition(Model model)
    {
        if (model is not IHasShape ihs)
            throw new DiagramsException("ShapeAnglePositionProvider requires an IHasShape model");
        
        var shape = ihs.GetShape();
        return shape.GetPointAtAngle(Angle)! + new Vector2(OffsetX, OffsetY);
    }
}