namespace Trivial.Domain.Events;

public record PointerEventArgs(float ClientX, float ClientY, long Button, long Buttons, bool CtrlKey, bool ShiftKey,
    bool AltKey, long PointerId, float Width, float Height, float Pressure, float TiltX, float TiltY,
    string PointerType, bool IsPrimary) : MouseEventArgs(ClientX, ClientY, Button, Buttons, CtrlKey, ShiftKey, AltKey);