using Trivial.Domain.Events;
using MouseEventArgs = Microsoft.AspNetCore.Components.Web.MouseEventArgs;

namespace Trivial.Graph.Extensions;

public static class EventsExtensions
{
    public static PointerEventArgs ToCore(this Microsoft.AspNetCore.Components.Web.PointerEventArgs E)
    {
        return new PointerEventArgs((float)E.ClientX, (float)E.ClientY, E.Button, E.Buttons, E.CtrlKey, E.ShiftKey, E.AltKey,
            E.PointerId, E.Width, E.Height, E.Pressure, E.TiltX, E.TiltY, E.PointerType, E.IsPrimary);
    }

    public static PointerEventArgs ToCore(this MouseEventArgs E)
    {
        return new PointerEventArgs((float)E.ClientX, (float)E.ClientY, E.Button, E.Buttons, E.CtrlKey, E.ShiftKey, E.AltKey,
            0, 0, 0, 0, 0, 0, string.Empty, false);
    }

    public static KeyboardEventArgs ToCore(this Microsoft.AspNetCore.Components.Web.KeyboardEventArgs E)
    {
        return new KeyboardEventArgs(E.Key, E.Code, E.Location, E.CtrlKey, E.ShiftKey, E.AltKey);
    }

    public static WheelEventArgs ToCore(this Microsoft.AspNetCore.Components.Web.WheelEventArgs E)
    {
        return new WheelEventArgs((float)E.ClientX, (float)E.ClientY, E.Button, E.Buttons, E.CtrlKey, E.ShiftKey, E.AltKey, (float)E.DeltaX,
            (float)E.DeltaY, (float)E.DeltaZ, E.DeltaMode);
    }
}