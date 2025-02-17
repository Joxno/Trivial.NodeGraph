namespace Trivial.Graph.Domain.Events;

public record WheelEventArgs(
    float ClientX,
    float ClientY,
    long Button,
    long Buttons,
    bool CtrlKey,
    bool ShiftKey,
    bool AltKey,
    float DeltaX,
    float DeltaY,
    float DeltaZ,
    long DeltaMode) : MouseEventArgs(ClientX, ClientY, Button, Buttons, CtrlKey, ShiftKey, AltKey);
