using Trivial.Domain.Models.Base;

namespace Trivial.Domain.Behaviors;

public class VirtualizationBehavior : BaseBehaviour
{
    public VirtualizationBehavior(Diagram Diagram) : base(Diagram)
    {
        base.Diagram.ZoomChanged += CheckVisibility;
        base.Diagram.PanChanged += CheckVisibility;
        base.Diagram.ContainerChanged += CheckVisibility;
    }

    private void CheckVisibility()
    {
        if (!Diagram.Options.Virtualization.Enabled)
            return;
        
        if (Diagram.Container == null)
            return;

        if (Diagram.Options.Virtualization.OnNodes)
        {
            foreach (var t_Node in Diagram.Nodes)
            {
                CheckVisibility(t_Node);
            }
        }

        if (Diagram.Options.Virtualization.OnGroups)
        {
            foreach (var t_Group in Diagram.Groups)
            {
                CheckVisibility(t_Group);
            }
        }

        if (Diagram.Options.Virtualization.OnLinks)
        {
            foreach (var t_Link in Diagram.Links)
            {
                CheckVisibility(t_Link);
            }
        }
    }

    private void CheckVisibility(Model Model)
    {
        if (Model is not IHasBounds t_Ihb)
            return;
        
        var t_Bounds = t_Ihb.GetBounds();
        if (t_Bounds == null)
            return;
        
        var t_Left = t_Bounds.Left * Diagram.Zoom + Diagram.Pan.X;
        var t_Top = t_Bounds.Top * Diagram.Zoom + Diagram.Pan.Y;
        var t_Right = t_Left + t_Bounds.Width * Diagram.Zoom;
        var t_Bottom = t_Top + t_Bounds.Height * Diagram.Zoom;
        Model.Visible = t_Right > 0 && t_Left < Diagram.Container!.Width && t_Bottom > 0 && t_Top < Diagram.Container.Height;
    }

    protected override void _OnDispose()
    {
        Diagram.ZoomChanged -= CheckVisibility;
        Diagram.PanChanged -= CheckVisibility;
        Diagram.ContainerChanged -= CheckVisibility;
    }
}