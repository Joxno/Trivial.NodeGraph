using System;
using Trivial.Domain.Anchors;
using Trivial.Domain.Models;
using Trivial.Domain.PathGenerators;
using Trivial.Domain.Routers;

namespace Trivial.Domain.Options;

public class DiagramLinkOptions
{
    private float m_SnappingRadius = 50;

    public Router DefaultRouter { get; set; } = new NormalRouter();
    public PathGenerator DefaultPathGenerator { get; set; } = new SmoothPathGenerator();
    public bool EnableSnapping { get; set; } = false;
    public bool RequireTarget { get; set; } = true;

    public float SnappingRadius
    {
        get => m_SnappingRadius;
        set
        {
            if (value <= 0)
                throw new ArgumentException($"SnappingRadius must be greater than zero");

            m_SnappingRadius = value;
        }
    }

    public LinkFactory Factory { get; set; } = (Diagram, Source, TargetAnchor) =>
    {
        Anchor t_SourceAnchor = Source switch
        {
            NodeModel t_Node => new ShapeIntersectionAnchor(t_Node),
            PortModel t_Port => new SinglePortAnchor(t_Port),
            _ => throw new NotImplementedException()
        };
        return new LinkModel(t_SourceAnchor, TargetAnchor);
    };

    public AnchorFactory TargetAnchorFactory { get; set; } = (Diagram, Link, Model) =>
    {
        return Model switch
        {
            NodeModel t_Node => new ShapeIntersectionAnchor(t_Node),
            PortModel t_Port => new SinglePortAnchor(t_Port),
            _ => throw new ArgumentOutOfRangeException(nameof(Model), Model, null)
        };
    };
}