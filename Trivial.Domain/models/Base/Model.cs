using System;

namespace Trivial.Graph.Domain.Models.Base;

public abstract class Model
{
    private bool m_Visible = true;
    
    protected Model() : this(Guid.NewGuid().ToString()) { }

    protected Model(string Id)
    {
        this.Id = Id;
    }

    public event Action<Model>? Changed;
    public event Action<Model>? VisibilityChanged;

    public string Id { get; }
    public bool Locked { get; set; }
    public bool Visible
    {
        get => m_Visible;
        set
        {
            if (m_Visible == value)
                return;

            m_Visible = value;
            VisibilityChanged?.Invoke(this);
        }
    }

    public virtual void Refresh() => Changed?.Invoke(this);
}
