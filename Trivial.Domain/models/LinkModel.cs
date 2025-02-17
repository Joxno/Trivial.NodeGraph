using Trivial.Graph.Domain.Anchors;
using Trivial.Graph.Domain.Models.Base;

namespace Trivial.Graph.Domain.Models;

public class LinkModel : BaseLinkModel
{
    public LinkModel(Anchor Source, Anchor Target) : base(Source, Target) { }

    public LinkModel(string Id, Anchor Source, Anchor Target) : base(Id, Source, Target) { }

    public LinkModel(PortModel SourcePort, PortModel TargetPort)
        : base(new SinglePortAnchor(SourcePort), new SinglePortAnchor(TargetPort)) { }

    public LinkModel(NodeModel SourceNode, NodeModel TargetNode)
        : base(new ShapeIntersectionAnchor(SourceNode), new ShapeIntersectionAnchor(TargetNode)) { }

    public LinkModel(string Id, PortModel SourcePort, PortModel TargetPort)
        : base(Id, new SinglePortAnchor(SourcePort), new SinglePortAnchor(TargetPort)) { }

    public LinkModel(string Id, NodeModel SourceNode, NodeModel TargetNode)
        : base(Id, new ShapeIntersectionAnchor(SourceNode), new ShapeIntersectionAnchor(TargetNode)) { }

    public string? Color { get; set; }
    public string? SelectedColor { get; set; }
    public float Width { get; set; } = 2;
}
