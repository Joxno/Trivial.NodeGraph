using System;
using Trivial.Domain.Anchors;
using Trivial.Domain.Models;
using Trivial.Domain.PathGenerators;
using Trivial.Domain.Routers;

namespace Trivial.Domain.Options;

public class DiagramLinkOptions
{
    private float _snappingRadius = 50;

    public Router DefaultRouter { get; set; } = new NormalRouter();
    public PathGenerator DefaultPathGenerator { get; set; } = new SmoothPathGenerator();
    public bool EnableSnapping { get; set; } = false;
    public bool RequireTarget { get; set; } = true;

    public float SnappingRadius
    {
        get => _snappingRadius;
        set
        {
            if (value <= 0)
                throw new ArgumentException($"SnappingRadius must be greater than zero");

            _snappingRadius = value;
        }
    }

    public LinkFactory Factory { get; set; } = (diagram, source, targetAnchor) =>
    {
        Anchor sourceAnchor = source switch
        {
            NodeModel node => new ShapeIntersectionAnchor(node),
            PortModel port => new SinglePortAnchor(port),
            _ => throw new NotImplementedException()
        };
        return new LinkModel(sourceAnchor, targetAnchor);
    };

    public AnchorFactory TargetAnchorFactory { get; set; } = (diagram, link, model) =>
    {
        return model switch
        {
            NodeModel node => new ShapeIntersectionAnchor(node),
            PortModel port => new SinglePortAnchor(port),
            _ => throw new ArgumentOutOfRangeException(nameof(model), model, null)
        };
    };
}