using Trivial.Graph.Domain.Anchors;
using Trivial.Graph.Domain.Models.Base;
using System.Linq;

namespace Trivial.Graph.Domain.Layers;

public class LinkLayer : BaseLayer<BaseLinkModel>
{
    public LinkLayer(Diagram Diagram) : base(Diagram) { }

    protected override void OnItemAdded(BaseLinkModel Link)
    {
        Link.Diagram = Diagram;
        HandleAnchor(Link, Link.Source, true);
        HandleAnchor(Link, Link.Target, true);
        Link.Refresh();

        Link.SourceChanged += OnLinkSourceChanged;
        Link.TargetChanged += OnLinkTargetChanged;
    }

    protected override void OnItemRemoved(BaseLinkModel Link)
    {
        Link.Diagram = null;
        HandleAnchor(Link, Link.Source, false);
        HandleAnchor(Link, Link.Target, false);
        Link.Refresh();

        Link.SourceChanged -= OnLinkSourceChanged;
        Link.TargetChanged -= OnLinkTargetChanged;
        
        Diagram.Controls.RemoveFor(Link);
        Remove(Link.Links.ToList());
    }

    private static void OnLinkSourceChanged(BaseLinkModel Link, Anchor Old, Anchor New)
    {
        HandleAnchor(Link, Old, Add: false);
        HandleAnchor(Link, New, Add: true);
    }

    private static void OnLinkTargetChanged(BaseLinkModel Link, Anchor Old, Anchor New)
    {
        HandleAnchor(Link, Old, Add: false);
        HandleAnchor(Link, New, Add: true);
    }

    private static void HandleAnchor(BaseLinkModel Link, Anchor Anchor, bool Add)
    {
        if (Add)
        {
            Anchor.Model?.AddLink(Link);
        }
        else
        {
            Anchor.Model?.RemoveLink(Link);
        }

        if (Anchor.Model is Model t_Model)
        {
            t_Model.Refresh();
        }
    }
}
