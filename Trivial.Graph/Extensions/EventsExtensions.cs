using Trivial.Domain.Events;
using MouseEventArgs = Microsoft.AspNetCore.Components.Web.MouseEventArgs;

namespace Trivial.Graph.Extensions;

public static class EventsExtensions
{
    public static PointerEventArgs ToCore(this Microsoft.AspNetCore.Components.Web.PointerEventArgs e)
    {
        return new PointerEventArgs((float)e.ClientX, (float)e.ClientY, e.Button, e.Buttons, e.CtrlKey, e.ShiftKey, e.AltKey,
            e.PointerId, e.Width, e.Height, e.Pressure, e.TiltX, e.TiltY, e.PointerType, e.IsPrimary);
    }

    public static PointerEventArgs ToCore(this MouseEventArgs e)
    {
        return new PointerEventArgs((float)e.ClientX, (float)e.ClientY, e.Button, e.Buttons, e.CtrlKey, e.ShiftKey, e.AltKey,
            0, 0, 0, 0, 0, 0, string.Empty, false);
    }

    public static KeyboardEventArgs ToCore(this Microsoft.AspNetCore.Components.Web.KeyboardEventArgs e)
    {
        return new KeyboardEventArgs(e.Key, e.Code, e.Location, e.CtrlKey, e.ShiftKey, e.AltKey);
    }

    public static WheelEventArgs ToCore(this Microsoft.AspNetCore.Components.Web.WheelEventArgs e)
    {
        return new WheelEventArgs((float)e.ClientX, (float)e.ClientY, e.Button, e.Buttons, e.CtrlKey, e.ShiftKey, e.AltKey, (float)e.DeltaX,
            (float)e.DeltaY, (float)e.DeltaZ, e.DeltaMode);
    }
}