using System.Numerics;
using Trivial.Graph.Domain.Geometry;
using Trivial.Graph.Domain.Models;
using Trivial.Graph.Domain.Models.Base;

namespace Trivial.Graph.Domain.Anchors;

public sealed class SinglePortAnchor : Anchor
{
    public SinglePortAnchor(PortModel Port) : base(Port)
    {
        this.Port = Port;
    }
    
    public PortModel Port { get; }
    public bool MiddleIfNoMarker { get; set; } = false;
    public bool UseShapeAndAlignment { get; set; } = true;

    public override Vector2? GetPosition(BaseLinkModel Link, Vector2[] Route)
    {
        if (!Port.Initialized)
            return null;

        if (MiddleIfNoMarker && ((Link.Source == this && Link.SourceMarker is null) || (Link.Target == this && Link.TargetMarker is null)))
            return Port.MiddlePosition;
        
        var t_Pt = Port.Position;
        if (UseShapeAndAlignment)
        {
            return Port.Alignment switch
            {
                PortAlignment.Top => Port.GetShape().GetPointAtAngle(270),
                PortAlignment.TopRight => Port.GetShape().GetPointAtAngle(315),
                PortAlignment.Right => Port.GetShape().GetPointAtAngle(0),
                PortAlignment.BottomRight => Port.GetShape().GetPointAtAngle(45),
                PortAlignment.Bottom => Port.GetShape().GetPointAtAngle(90),
                PortAlignment.BottomLeft => Port.GetShape().GetPointAtAngle(135),
                PortAlignment.Left => Port.GetShape().GetPointAtAngle(180),
                PortAlignment.TopLeft => Port.GetShape().GetPointAtAngle(225),
                _ => null,
            };
        }

        return Port.Alignment switch
        {
            PortAlignment.Top => new Vector2(t_Pt.X + Port.Size.Width / 2, t_Pt.Y),
            PortAlignment.TopRight => new Vector2(t_Pt.X + Port.Size.Width, t_Pt.Y),
            PortAlignment.Right => new Vector2(t_Pt.X + Port.Size.Width, t_Pt.Y + Port.Size.Height / 2),
            PortAlignment.BottomRight => new Vector2(t_Pt.X + Port.Size.Width, t_Pt.Y + Port.Size.Height),
            PortAlignment.Bottom => new Vector2(t_Pt.X + Port.Size.Width / 2, t_Pt.Y + Port.Size.Height),
            PortAlignment.BottomLeft => new Vector2(t_Pt.X, t_Pt.Y + Port.Size.Height),
            PortAlignment.Left => new Vector2(t_Pt.X, t_Pt.Y + Port.Size.Height / 2),
            _ => t_Pt,
        };
    }

    public override Vector2? GetPlainPosition() => Port.MiddlePosition;
}
