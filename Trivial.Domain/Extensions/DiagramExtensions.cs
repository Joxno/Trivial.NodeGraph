using Trivial.Domain.Geometry;
using Trivial.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace Trivial.Domain.Extensions;

public static class DiagramExtensions
{
    public static Rectangle GetBounds(this IEnumerable<NodeModel> nodes)
    {
        if (!nodes.Any())
            return Rectangle.Zero;

        var minX = float.MaxValue;
        var maxX = float.MinValue;
        var minY = float.MaxValue;
        var maxY = float.MinValue;

        foreach (var node in nodes)
        {
            if (node.Size == null) // Ignore nodes that didn't get a size yet
                continue;

            var trX = node.Position.X + node.Size!.Width;
            var bY = node.Position.Y + node.Size.Height;

            if (node.Position.X < minX)
            {
                minX = node.Position.X;
            }
            if (trX > maxX)
            {
                maxX = trX;
            }
            if (node.Position.Y < minY)
            {
                minY = node.Position.Y;
            }
            if (bY > maxY)
            {
                maxY = bY;
            }
        }

        return new Rectangle(minX, minY, maxX, maxY);
    }
}
