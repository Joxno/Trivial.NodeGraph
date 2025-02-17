using Trivial.Domain.Events;

using System;

namespace Trivial.Domain.Behaviors;

public class ZoomBehavior : BaseBehaviour
{
    public ZoomBehavior(Diagram Diagram) : base(Diagram)
    {
        base.Diagram.Wheel += Diagram_Wheel;
    }

    private void Diagram_Wheel(WheelEventArgs E)
    {
        if (Diagram.Container == null || E.DeltaY == 0)
            return;

        if (!Diagram.Options.Zoom.Enabled)
            return;

        var t_Scale = Diagram.Options.Zoom.ScaleFactor;
        var t_OldZoom = Diagram.Zoom;
        var t_DeltaY = Diagram.Options.Zoom.Inverse ? E.DeltaY * -1 : E.DeltaY;
        var t_NewZoom = t_DeltaY > 0 ? t_OldZoom * t_Scale : t_OldZoom / t_Scale;
        t_NewZoom = Math.Clamp(t_NewZoom, Diagram.Options.Zoom.Minimum, Diagram.Options.Zoom.Maximum);

        if (t_NewZoom < 0 || t_NewZoom == Diagram.Zoom)
            return;

        // Other algorithms (based only on the changes in the zoom) don't work for our case
        // This solution is taken as is from react-diagrams (ZoomCanvasAction)
        var t_ClientWidth = Diagram.Container.Width;
        var t_ClientHeight = Diagram.Container.Height;
        var t_WidthDiff = t_ClientWidth * t_NewZoom - t_ClientWidth * t_OldZoom;
        var t_HeightDiff = t_ClientHeight * t_NewZoom - t_ClientHeight * t_OldZoom;
        var t_ClientX = E.ClientX - Diagram.Container.Left;
        var t_ClientY = E.ClientY - Diagram.Container.Top;
        var t_XFactor = (t_ClientX - Diagram.Pan.X) / t_OldZoom / t_ClientWidth;
        var t_YFactor = (t_ClientY - Diagram.Pan.Y) / t_OldZoom / t_ClientHeight;
        var t_NewPanX = Diagram.Pan.X - t_WidthDiff * t_XFactor;
        var t_NewPanY = Diagram.Pan.Y - t_HeightDiff * t_YFactor;

        Diagram.Batch(() =>
        {
            Diagram.SetPan(t_NewPanX, t_NewPanY);
            Diagram.SetZoom(t_NewZoom);
        });
    }

    protected override void _OnDispose()
    {
        Diagram.Wheel -= Diagram_Wheel;
    }
}
