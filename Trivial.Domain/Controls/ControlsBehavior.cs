using Trivial.Graph.Domain.Events;
using Trivial.Graph.Domain.Models.Base;

namespace Trivial.Graph.Domain.Controls;

public class ControlsBehavior : BaseBehaviour
{
    public ControlsBehavior(Diagram Diagram) : base(Diagram)
    {
        base.Diagram.PointerEnter += OnPointerEnter;
        base.Diagram.PointerLeave += OnPointerLeave;
        base.Diagram.PointerDoubleClick += OnPointerDoubleClick;
        base.Diagram.SelectionChanged += OnSelectionChanged;
    }

    private void OnPointerDoubleClick(Model? Model, PointerEventArgs Args) => Diagram.Controls.GetFor(Model).Then(C => {
        if (!C.HasControlType(ControlsType.OnSelection))
            return;

        C.Show();
    });

    private void OnSelectionChanged(SelectableModel Model) => Diagram.Controls.GetFor(Model).Then(C => {
        if (!C.HasControlType(ControlsType.OnSelection))
            return;

        if (Model.Selected)
            C.Show();
        else
            C.Hide();
    });

    private void OnPointerEnter(Model? Model, PointerEventArgs E) => Model.ToMaybe().Bind(Diagram.Controls.GetFor).Then(C => {
        if(!C.HasControlType(ControlsType.OnHover))
            return;

        C.Show();
    });

    private void OnPointerLeave(Model? Model, PointerEventArgs E) => Model.ToMaybe().Bind(Diagram.Controls.GetFor).Then(C => {
        if(!C.HasControlType(ControlsType.OnHover))
            return;

        C.Hide();
    });

    protected override void _OnDispose()
    {
        Diagram.PointerEnter -= OnPointerEnter;
        Diagram.PointerLeave -= OnPointerLeave;
        Diagram.SelectionChanged -= OnSelectionChanged;
    }
}