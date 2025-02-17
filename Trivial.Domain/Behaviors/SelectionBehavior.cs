using Trivial.Graph.Domain.Models.Base;
using Trivial.Graph.Domain.Events;

namespace Trivial.Graph.Domain.Behaviors;

public class SelectionBehavior : BaseBehaviour
{
    public SelectionBehavior(Diagram Diagram) : base(Diagram) {}

    protected override void _OnPointerDown(Maybe<Model> Model, PointerEventArgs E)
    {
        var t_CtrlKey = E.CtrlKey;
        switch (Model!.ValueOr(null))
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
}