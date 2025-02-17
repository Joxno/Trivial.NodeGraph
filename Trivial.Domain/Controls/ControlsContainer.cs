using System;
using System.Collections;
using System.Collections.Generic;
using Trivial.Graph.Domain.Models.Base;

namespace Trivial.Graph.Domain.Controls;

public class ControlsContainer : IReadOnlyList<Control>
{
    private readonly List<Control> m_Controls = new(4);

    public event Action<Model>? VisibilityChanged;
    public event Action<Model>? ControlsChanged;

    public ControlsContainer(Model Model, ControlsType Type = ControlsType.OnSelection)
    {
        this.Model = Model;
        this.Type = Type;
    }

    public Model Model { get; }
    public ControlsType Type { get; set; }
    public bool Visible { get; private set; }

    public void Show()
    {
        if (Visible)
            return;
        
        Visible = true;
        VisibilityChanged?.Invoke(Model);
    }

    public void Hide()
    {
        if (!Visible)
            return;
        
        Visible = false;
        VisibilityChanged?.Invoke(Model);
    }

    public ControlsContainer Add(Control Control)
    {
        m_Controls.Add(Control);
        ControlsChanged?.Invoke(Model);
        return this;
    }

    public ControlsContainer Remove(Control Control)
    {
        if (m_Controls.Remove(Control))
        {
            ControlsChanged?.Invoke(Model);
        }

        return this;
    }

    public int Count => m_Controls.Count;
    public Control this[int Index] => m_Controls[Index];
    public IEnumerator<Control> GetEnumerator() => m_Controls.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => m_Controls.GetEnumerator();
}