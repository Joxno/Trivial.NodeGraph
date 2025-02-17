using Trivial.Graph.Domain.Extensions;
using Trivial.Graph.Domain.Geometry;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Trivial.Graph.Domain.Models;

public class GroupModel : NodeModel
{
    private readonly List<NodeModel> m_Children;

    public GroupModel(IEnumerable<NodeModel> Children, byte Padding = 30, bool AutoSize = true)
    {
        m_Children = new List<NodeModel>();
        this.Padding = Padding;
        this.AutoSize = AutoSize;
        Initialize(Children);
    }

    public IReadOnlyList<NodeModel> Children => m_Children;
    public byte Padding { get; }
    public bool AutoSize { get; }

    public void AddChild(NodeModel Child)
    {
        m_Children.Add(Child);
        Child.Group = this;
        Child.SizeChanged += OnNodeChanged;
        Child.Moving += OnNodeChanged;

        if (UpdateDimensions())
        {
            Refresh();
        }
    }

    public void RemoveChild(NodeModel Child)
    {
        if (!m_Children.Remove(Child))
            return;

        Child.Group = null;
        Child.SizeChanged -= OnNodeChanged;
        Child.Moving -= OnNodeChanged;

        if (UpdateDimensions())
        {
            Refresh();
            RefreshLinks();
        }
    }

    public override void SetPosition(float X, float Y)
    {
        var t_DeltaX = X - Position.X;
        var t_DeltaY = Y - Position.Y;
        base.SetPosition(X, Y);

        foreach (var t_Node in Children)
        {
            t_Node.UpdatePositionSilently(t_DeltaX, t_DeltaY);
            t_Node.RefreshLinks();
        }

        Refresh();
        RefreshLinks();
    }

    public override void UpdatePositionSilently(float DeltaX, float DeltaY)
    {
        base.UpdatePositionSilently(DeltaX, DeltaY);

        foreach (var t_Child in Children)
            t_Child.UpdatePositionSilently(DeltaX, DeltaY);

        Refresh();
    }

    public void Ungroup()
    {
        foreach (var t_Child in Children)
        {
            t_Child.Group = null;
            t_Child.SizeChanged -= OnNodeChanged;
            t_Child.Moving -= OnNodeChanged;
        }

        m_Children.Clear();
    }

    private void Initialize(IEnumerable<NodeModel> Children)
    {
        foreach (var t_Child in Children)
        {
            m_Children.Add(t_Child);
            t_Child.Group = this; 
            t_Child.SizeChanged += OnNodeChanged;
            t_Child.Moving += OnNodeChanged;
        }

        UpdateDimensions();
    }

    private void OnNodeChanged(NodeModel Node)
    {
        if (UpdateDimensions())
        {
            Refresh();
        }
    }

    private bool UpdateDimensions()
    {
        if (Children.Count == 0)
            return true;

        if (Children.Any(N => N.Size == null))
            return false;

        var t_Bounds = Children.GetBounds();

        var t_NewPosition = new Vector2(t_Bounds.Left - Padding, t_Bounds.Top - Padding);
        if (!Position.Equals(t_NewPosition))
        {
            Position = t_NewPosition;
            TriggerMoving();
        }

        Size = new Size(t_Bounds.Width + Padding * 2, t_Bounds.Height + Padding * 2);
        return true;
    }
}
