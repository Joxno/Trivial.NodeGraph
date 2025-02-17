namespace Trivial.Graph.Domain.Events;

public record TouchEventArgs(TouchPoint[] ChangedTouches, bool CtrlKey, bool ShiftKey, bool AltKey);
public record TouchPoint(long Identifier, float ClientX, float ClientY);
