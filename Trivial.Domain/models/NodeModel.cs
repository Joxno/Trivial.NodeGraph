using Trivial.Domain.Geometry;
using Trivial.Domain.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Trivial.Domain.Models;

public class NodeModel : MovableModel, IHasBounds, IHasShape, ILinkable
{
    private readonly List<PortModel> m_Ports = new();
    private readonly List<BaseLinkModel> m_Links = new();
    private Size m_Size;

    public event Action<NodeModel>? SizeChanged;
    public event Action<NodeModel>? Moving;

    public NodeModel(Vector2? Position = null) : base(Position) {}
    public NodeModel(string Id, Vector2? Position = null) : base(Id, Position) {}

    public Size Size
    {
        get => m_Size;
        set
        {
            if (value == m_Size) return;

            m_Size = value;
            SizeChanged?.Invoke(this);
        }
    }
    public bool ControlledSize { get; init; }

    public GroupModel? Group { get; internal set; }
    public string? Title { get; set; }

    public IReadOnlyList<PortModel> Ports => m_Ports;
    public IReadOnlyList<BaseLinkModel> Links => m_Links;
    public IEnumerable<BaseLinkModel> PortLinks => Ports.SelectMany(P => P.Links);

    #region Ports

    public PortModel AddPort(PortModel Port)
    {
        m_Ports.Add(Port);
        return Port;
    }

    public PortModel AddPort(PortAlignment Alignment = PortAlignment.Bottom)
        => AddPort(new PortModel(this, Position, new(), Alignment));

    public PortModel? GetPort(PortAlignment Alignment) => Ports.FirstOrDefault(P => P.Alignment == Alignment);

    public T? GetPort<T>(PortAlignment Alignment) where T : PortModel => (T?)GetPort(Alignment);

    public bool RemovePort(PortModel Port) => m_Ports.Remove(Port);

    #endregion

    #region Refreshing

    public void RefreshAll()
    {
        Refresh();
        m_Ports.ForEach(P => P.RefreshAll());
    }

    public void RefreshLinks()
    {
        foreach (var t_Link in Links)
        {
            t_Link.Refresh();
            t_Link.RefreshLinks();
        }
    }

    public void ReinitializePorts()
    {
        foreach (var t_Port in Ports)
        {
            t_Port.Initialized = false;
            t_Port.Refresh();
        }
    }

    #endregion

    public override void SetPosition(float X, float Y)
    {
        var t_DeltaX = X - Position.X;
        var t_DeltaY = Y - Position.Y;
        base.SetPosition(X, Y);

        UpdatePortPositions(t_DeltaX, t_DeltaY);
        Refresh();
        RefreshLinks();
        Moving?.Invoke(this);
    }

    public virtual void UpdatePositionSilently(float DeltaX, float DeltaY)
    {
        base.SetPosition(Position.X + DeltaX, Position.Y + DeltaY);
        UpdatePortPositions(DeltaX, DeltaY);
        Refresh();
    }

    public Rectangle? GetBounds() => GetBounds(false);

    public Rectangle? GetBounds(bool IncludePorts)
    {
        if (!Size.IsVisibleSize()) return null;
        if (!IncludePorts) return new Rectangle(Position, Size);

        var t_LeftPort = GetPort(PortAlignment.Left);
        var t_TopPort = GetPort(PortAlignment.Top);
        var t_RightPort = GetPort(PortAlignment.Right);
        var t_BottomPort = GetPort(PortAlignment.Bottom);

        var t_Left = t_LeftPort == null ? Position.X : MathF.Min(Position.X, t_LeftPort.Position.X);
        var t_Top = t_TopPort == null ? Position.Y : MathF.Min(Position.Y, t_TopPort.Position.Y);
        var t_Right = t_RightPort == null
            ? Position.X + Size!.Width
            : MathF.Max(t_RightPort.Position.X + t_RightPort.Size.Width, Position.X + Size!.Width);
        var t_Bottom = t_BottomPort == null
            ? Position.Y + Size!.Height
            : MathF.Max(t_BottomPort.Position.Y + t_BottomPort.Size.Height, Position.Y + Size!.Height);

        return new Rectangle(t_Left, t_Top, t_Right, t_Bottom);
    }

    public virtual IShape GetShape() => Shapes.Rectangle(this);

    public virtual bool CanAttachTo(ILinkable Other) => Other is not PortModel && Other is not BaseLinkModel;

    private void UpdatePortPositions(float DeltaX, float DeltaY)
    {
        // Save some JS calls and update ports directly here
        foreach (var t_Port in m_Ports)
        {
            t_Port.Position = new Vector2(t_Port.Position.X + DeltaX, t_Port.Position.Y + DeltaY);
            t_Port.RefreshLinks();
        }
    }

    protected void TriggerMoving()
    {
        Moving?.Invoke(this);
    }

    void ILinkable.AddLink(BaseLinkModel Link) => m_Links.Add(Link);

    void ILinkable.RemoveLink(BaseLinkModel Link) => m_Links.Remove(Link);
}