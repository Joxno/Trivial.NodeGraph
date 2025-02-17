using Trivial.Domain.Models;
using System.Linq;

namespace Trivial.Domain.Layers;

public class NodeLayer : BaseLayer<NodeModel>
{
    public NodeLayer(Diagram diagram) : base(diagram) { }

    protected override void OnItemRemoved(NodeModel node)
    {
        Diagram.Links.Remove(node.PortLinks.ToList());
        Diagram.Links.Remove(node.Links.ToList());
        node.Group?.RemoveChild(node);
        Diagram.Controls.RemoveFor(node);
    }
}
