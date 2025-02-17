using Trivial.Graph.Domain.Events;
using Trivial.Graph.Domain.Utils;

namespace Trivial.Graph.Domain.Behaviors;

public class KeyboardShortcutsBehavior : BaseBehaviour
{
    private readonly Dictionary<string, Func<Diagram, ValueTask>> m_Shortcuts;

    public KeyboardShortcutsBehavior(Diagram Diagram) : base(Diagram)
    {
        m_Shortcuts = new Dictionary<string, Func<Diagram, ValueTask>>();
        SetShortcut("Delete", false, false, false, KeyboardShortcutsDefaults.DeleteSelection);
        SetShortcut("g", true, false, true, KeyboardShortcutsDefaults.Grouping);
    }

    public void SetShortcut(string Key, bool Ctrl, bool Shift, bool Alt, Func<Diagram, ValueTask> Action)
    {
        var t_K = KeysUtils.GetStringRepresentation(Ctrl, Shift, Alt, Key);
        m_Shortcuts[t_K] = Action;
    }

    public bool RemoveShortcut(string Key, bool Ctrl, bool Shift, bool Alt)
    {
        var t_K = KeysUtils.GetStringRepresentation(Ctrl, Shift, Alt, Key);
        return m_Shortcuts.Remove(t_K);
    }

    protected override async void _OnKeyDown(KeyboardEventArgs E)
    {
        var t_K = KeysUtils.GetStringRepresentation(E.CtrlKey, E.ShiftKey, E.AltKey, E.Key);
        if (m_Shortcuts.TryGetValue(t_K, out var t_Action))
        {
            await t_Action(Diagram);
        }
    }
}
