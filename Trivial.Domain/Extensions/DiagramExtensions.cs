using Trivial.Domain.Geometry;
using Trivial.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace Trivial.Domain.Extensions;

public static class DiagramExtensions
{
    public static Rectangle GetBounds(this IEnumerable<NodeModel> Nodes)
    {
        if (!Nodes.Any())
            return Rectangle.Zero;

        var t_MinX = float.MaxValue;
        var t_MaxX = float.MinValue;
        var t_MinY = float.MaxValue;
        var t_MaxY = float.MinValue;

        foreach (var t_Node in Nodes)
        {
            if (t_Node.Size == null) // Ignore nodes that didn't get a size yet
                continue;

            var t_TrX = t_Node.Position.X + t_Node.Size!.Width;
            var t_BY = t_Node.Position.Y + t_Node.Size.Height;

            if (t_Node.Position.X < t_MinX)
            {
                t_MinX = t_Node.Position.X;
            }
            if (t_TrX > t_MaxX)
            {
                t_MaxX = t_TrX;
            }
            if (t_Node.Position.Y < t_MinY)
            {
                t_MinY = t_Node.Position.Y;
            }
            if (t_BY > t_MaxY)
            {
                t_MaxY = t_BY;
            }
        }

        return new Rectangle(t_MinX, t_MinY, t_MaxX, t_MaxY);
    }
}
