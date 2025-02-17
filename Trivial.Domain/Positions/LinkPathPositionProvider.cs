using Trivial.Domain.Geometry;
using Trivial.Domain.Models.Base;
using System;
using System.Numerics;

namespace Trivial.Domain.Positions;

public class LinkPathPositionProvider : IPositionProvider
{
    public LinkPathPositionProvider(float distance, float offsetX = 0, float offsetY = 0)
    {
        Distance = distance;
        OffsetX = offsetX;
        OffsetY = offsetY;
    }

    public float Distance { get; }
    public float OffsetX { get; }
    public float OffsetY { get; }

    public Vector2? GetPosition(Model model)
    {
        if (model is not BaseLinkModel link)
            throw new DiagramsException("LinkPathPositionProvider requires a link model");
        
        if (link.PathGeneratorResult == null)
            return null;
            
        var totalLength = link.PathGeneratorResult.FullPath.Length;
        var length = Distance switch
        {
            >= 0 and <= 1 => Distance * totalLength,
            > 1 => Distance,
            < 0 => totalLength + Distance,
            _ => throw new NotImplementedException()
        };

        var pt = link.PathGeneratorResult.FullPath.GetPointAtLength(length);
        return new Vector2((float)pt.X + OffsetX, (float)pt.Y + OffsetY);
    }
}