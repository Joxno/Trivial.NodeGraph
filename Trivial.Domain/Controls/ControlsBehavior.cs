using Trivial.Graph.Domain.Events;
using Trivial.Graph.Domain.Models.Base;

namespace Trivial.Graph.Domain.Controls;

public class ControlsBehavior : BaseBehaviour
{
    public ControlsBehavior(Diagram Diagram) : base(Diagram)
    {
        base.Diagram.PointerEnter += OnPointerEnter;
        base.Diagram.PointerLeave += OnPointerLeave;
        base.Diagram.SelectionChanged += OnSelectionChanged;
    }

    private void OnSelectionChanged(SelectableModel Model)
    {
        var t_Controls = Diagram.Controls.GetFor(Model);
        if (t_Controls is not { Type: ControlsType.OnSelection })
            return;

        if (Model.Selected)
        {
            t_Controls.Show();
        }
        else
        {
            t_Controls.Hide();
        }
    }

    private void OnPointerEnter(Model? Model, PointerEventArgs E)
    {
        if (Model == null)
            return;
        
        var t_Controls = Diagram.Controls.GetFor(Model);
        if (t_Controls is not { Type: ControlsType.OnHover })
            return;
        
        t_Controls.Show();
    }

    private void OnPointerLeave(Model? Model, PointerEventArgs E)
    {
        if (Model == null)
            return;
        
        var t_Controls = Diagram.Controls.GetFor(Model);
        if (t_Controls is not { Type: ControlsType.OnHover })
            return;
        
        t_Controls.Hide();
    }

    protected override void _OnDispose()
    {
        Diagram.PointerEnter -= OnPointerEnter;
        Diagram.PointerLeave -= OnPointerLeave;
        Diagram.SelectionChanged -= OnSelectionChanged;
    }
}