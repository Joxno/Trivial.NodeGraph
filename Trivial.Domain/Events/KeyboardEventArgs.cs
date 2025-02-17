namespace Trivial.Domain.Events;

public record KeyboardEventArgs(string Key, string Code, float Location, bool CtrlKey, bool ShiftKey, bool AltKey);
