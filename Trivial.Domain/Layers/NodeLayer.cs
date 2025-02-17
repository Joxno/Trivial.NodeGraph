using Trivial.Graph.Domain.Models;
using System.Linq;

namespace Trivial.Graph.Domain.Layers;

public class NodeLayer : BaseLayer<NodeModel>
{
    public NodeLayer(Diagram Diagram) : base(Diagram) { }

    protected override void OnItemRemoved(NodeModel Node)
    {
        Diagram.Links.Remove(Node.PortLinks.ToList());
        Diagram.Links.Remove(Node.Links.ToList());
        Node.Group?.RemoveChild(Node);
        Diagram.Controls.RemoveFor(Node);
    }
}
