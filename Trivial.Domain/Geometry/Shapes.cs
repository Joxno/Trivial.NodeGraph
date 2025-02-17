using System.Numerics;
using Trivial.Graph.Domain.Models;

namespace Trivial.Graph.Domain.Geometry;

public static class Shapes
{
    public static IShape Rectangle(NodeModel Node) => Rectangle(Node.Position, Node.Size!);

    public static IShape Circle(NodeModel Node) => Circle(Node.Position, Node.Size!);

    public static IShape Ellipse(NodeModel Node) => Ellipse(Node.Position, Node.Size!);

    public static IShape Rectangle(PortModel Port) => Rectangle(Port.Position, Port.Size!);

    public static IShape Circle(PortModel Port) => Circle(Port.Position, Port.Size!);

    public static IShape Ellipse(PortModel Port) => Ellipse(Port.Position, Port.Size!);
    
    private static IShape Rectangle(Vector2 Position, Size Size) => new Rectangle(Position, Size);

    private static IShape Circle(Vector2 Position, Size Size)
    {
        var t_HalfWidth = Size.Width / 2;
        var t_CenterX = Position.X + t_HalfWidth;
        var t_CenterY = Position.Y + Size.Height / 2;
        return new Ellipse(t_CenterX, t_CenterY, t_HalfWidth, t_HalfWidth);
    }

    private static IShape Ellipse(Vector2 Position, Size Size)
    {
        var t_HalfWidth = Size.Width / 2;
        var t_HalfHeight = Size.Height / 2;
        var t_CenterX = Position.X + t_HalfWidth;
        var t_CenterY = Position.Y + t_HalfHeight;
        return new Ellipse(t_CenterX, t_CenterY, t_HalfWidth, t_HalfHeight);
    }
}
