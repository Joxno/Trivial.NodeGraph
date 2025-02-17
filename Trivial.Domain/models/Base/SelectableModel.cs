using System;

namespace Trivial.Graph.Domain.Models.Base;

public abstract class SelectableModel : Model
{
    private int m_Order;

    public event Action<SelectableModel>? OrderChanged;

    protected SelectableModel() { }

    protected SelectableModel(string Id) : base(Id) { }

    public bool Selected { get; internal set; }
    public int Order
    {
        get => m_Order;
        set
        {
            if (value == Order)
                return;

            m_Order = value;
            OrderChanged?.Invoke(this);
        }
    }
}
