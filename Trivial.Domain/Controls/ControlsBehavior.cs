using Trivial.Domain.Events;
using Trivial.Domain.Models.Base;

namespace Trivial.Domain.Controls;

public class ControlsBehavior : Behavior
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

    public override void Dispose()
    {
        Diagram.PointerEnter -= OnPointerEnter;
        Diagram.PointerLeave -= OnPointerLeave;
        Diagram.SelectionChanged -= OnSelectionChanged;
    }
}