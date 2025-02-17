using Trivial.Domain.Models.Base;
using Trivial.Domain.Events;

namespace Trivial.Domain.Behaviors;

public class SelectionBehavior : Behavior
{
    public SelectionBehavior(Diagram Diagram) : base(Diagram)
    {
        base.Diagram.PointerDown += OnPointerDown;
    }

    private void OnPointerDown(Model? Model, PointerEventArgs E)
    {
        var t_CtrlKey = E.CtrlKey;
        switch (Model)
        {
            case null:
                Diagram.UnselectAll();
                break;
            case SelectableModel t_Sm when t_CtrlKey && t_Sm.Selected:
                Diagram.UnselectModel(t_Sm);
                break;
            case SelectableModel t_Sm:
            {
                if (!t_Sm.Selected)
                {
                    Diagram.SelectModel(t_Sm, !t_CtrlKey || !Diagram.Options.AllowMultiSelection);
                }

                break;
            }
        }
    }

    public override void Dispose()
    {
        Diagram.PointerDown -= OnPointerDown;
    }
}
