using Trivial.Graph.Domain.Extensions;
using Trivial.Graph.Domain.Models.Base;

namespace Trivial.Graph.Domain.handlers;

public class ZoomHandler(Diagram Graph)
{
    public event Action? ZoomChanged;
    
    public void ZoomToFit(float Margin = 10)
    {
        if (Graph.Container == null || Graph.Nodes.Count == 0)
            return;

        var t_SelectedNodes = Graph.Nodes.Where(S => S.Selected);
        var t_NodesToUse = t_SelectedNodes.Any() ? t_SelectedNodes : Graph.Nodes;
        var t_Bounds = t_NodesToUse.GetBounds();
        var t_Width = t_Bounds.Width + 2 * Margin;
        var t_Height = t_Bounds.Height + 2 * Margin;
        var t_MinX = t_Bounds.Left - Margin;
        var t_MinY = t_Bounds.Top - Margin;

        Graph.SuspendRefresh = true;

        var t_Xf = Graph.Container.Width / t_Width;
        var t_Yf = Graph.Container.Height / t_Height;
        SetZoom(MathF.Min(t_Xf, t_Yf));

        var t_Nx = Graph.Container.Left + Graph.Pan.X + t_MinX * Graph.Zoom;
        var t_Ny = Graph.Container.Top + Graph.Pan.Y + t_MinY * Graph.Zoom;
        Graph.UpdatePan(Graph.Container.Left - t_Nx, Graph.Container.Top - t_Ny);

        Graph.SuspendRefresh = false;
        Graph.Refresh();
    }

    public void SetZoom(float NewZoom)
    {
        if (NewZoom <= 0)
            throw new ArgumentException($"{nameof(NewZoom)} cannot be equal or lower than 0");

        if (NewZoom < Graph.Options.Zoom.Minimum)
            NewZoom = Graph.Options.Zoom.Minimum;

        Graph.Zoom = NewZoom;
        ZoomChanged?.Invoke();
        Graph.Refresh();
    }
}