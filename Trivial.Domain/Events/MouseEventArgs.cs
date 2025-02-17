namespace Trivial.Graph.Domain.Events;

public record MouseEventArgs(float ClientX, float ClientY, long Button, long Buttons, bool CtrlKey, bool ShiftKey, bool AltKey);
