using System;

namespace Trivial.Domain;

public abstract class Behavior : IDisposable
{
    public Behavior(Diagram diagram)
    {
        Diagram = diagram;
    }

    protected Diagram Diagram { get; }

    public abstract void Dispose();
}
