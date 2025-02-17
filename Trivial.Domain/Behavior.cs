using System;

namespace Trivial.Domain;

public abstract class Behavior : IDisposable
{
    public Behavior(Diagram Diagram)
    {
        this.Diagram = Diagram;
    }

    protected Diagram Diagram { get; }

    public abstract void Dispose();
}
