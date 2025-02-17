using Trivial.Domain.Geometry;
using Trivial.Domain.Models.Base;
using System;
using System.Numerics;

namespace Trivial.Domain.Positions;

public class LinkPathPositionProvider : IPositionProvider
{
    public LinkPathPositionProvider(float Distance, float OffsetX = 0, float OffsetY = 0)
    {
        this.Distance = Distance;
        this.OffsetX = OffsetX;
        this.OffsetY = OffsetY;
    }

    public float Distance { get; }
    public float OffsetX { get; }
    public float OffsetY { get; }

    public Vector2? GetPosition(Model Model)
    {
        if (Model is not BaseLinkModel t_Link)
            throw new DiagramsException("LinkPathPositionProvider requires a link model");
        
        if (t_Link.PathGeneratorResult == null)
            return null;
            
        var t_TotalLength = t_Link.PathGeneratorResult.FullPath.Length;
        var t_Length = Distance switch
        {
            >= 0 and <= 1 => Distance * t_TotalLength,
            > 1 => Distance,
            < 0 => t_TotalLength + Distance,
            _ => throw new NotImplementedException()
        };

        var t_Pt = t_Link.PathGeneratorResult.FullPath.GetPointAtLength(t_Length);
        return new Vector2((float)t_Pt.X + OffsetX, (float)t_Pt.Y + OffsetY);
    }
}