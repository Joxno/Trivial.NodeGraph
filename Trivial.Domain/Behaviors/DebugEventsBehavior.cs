using Trivial.Domain.Models;
using Trivial.Domain.Models.Base;
using System;

namespace Trivial.Domain.Behaviors;

public class DebugEventsBehavior : BaseBehaviour
{
    public DebugEventsBehavior(Diagram Diagram) : base(Diagram)
    {
        base.Diagram.Changed += Diagram_Changed;
        base.Diagram.ContainerChanged += Diagram_ContainerChanged;
        base.Diagram.PanChanged += Diagram_PanChanged;
        base.Diagram.Nodes.Added += Nodes_Added;
        base.Diagram.Nodes.Removed += Nodes_Removed;
        base.Diagram.Links.Added += Links_Added;
        base.Diagram.Links.Removed += Links_Removed;
        base.Diagram.Groups.Added += Diagram_GroupAdded;
        base.Diagram.Groups.Removed += Diagram_GroupRemoved;
        base.Diagram.SelectionChanged += Diagram_SelectionChanged;
        base.Diagram.ZoomChanged += Diagram_ZoomChanged;
    }

    private void Diagram_ZoomChanged()
    {
        Console.WriteLine($"ZoomChanged, Zoom={Diagram.Zoom}");
    }

    private void Diagram_SelectionChanged(SelectableModel Obj)
    {
        Console.WriteLine($"SelectionChanged, Model={Obj.GetType().Name}, Selected={Obj.Selected}");
    }

    private void Links_Removed(BaseLinkModel Obj)
    {
        Console.WriteLine($"Links.Removed, Links=[{Obj}]");
    }

    private void Links_Added(BaseLinkModel Obj)
    {
        Console.WriteLine($"Links.Added, Links=[{Obj}]");
    }

    private void Nodes_Removed(NodeModel Obj)
    {
        Console.WriteLine($"Nodes.Removed, Nodes=[{Obj}]");
    }

    private void Nodes_Added(NodeModel Obj)
    {
        Console.WriteLine($"Nodes.Added, Nodes=[{Obj}]");
    }

    private void Diagram_PanChanged()
    {
        Console.WriteLine($"PanChanged, Pan={Diagram.Pan}");
    }

    private void Diagram_GroupRemoved(GroupModel Obj)
    {
        Console.WriteLine($"GroupRemoved, Id={Obj.Id}");
    }

    private void Diagram_GroupAdded(GroupModel Obj)
    {
        Console.WriteLine($"GroupAdded, Id={Obj.Id}");
    }

    private void Diagram_ContainerChanged()
    {
        Console.WriteLine($"ContainerChanged, Container={Diagram.Container}");
    }

    private void Diagram_Changed()
    {
        Console.WriteLine("Changed");
    }

    protected override void _OnDispose()
    {
        Diagram.Changed -= Diagram_Changed;
        Diagram.ContainerChanged -= Diagram_ContainerChanged;
        Diagram.PanChanged -= Diagram_PanChanged;
        Diagram.Nodes.Added -= Nodes_Added;
        Diagram.Nodes.Removed -= Nodes_Removed;
        Diagram.Links.Added -= Links_Added;
        Diagram.Links.Removed -= Links_Removed;
        Diagram.Groups.Added -= Diagram_GroupAdded;
        Diagram.Groups.Removed -= Diagram_GroupRemoved;
        Diagram.SelectionChanged -= Diagram_SelectionChanged;
        Diagram.ZoomChanged -= Diagram_ZoomChanged;
    }
}
