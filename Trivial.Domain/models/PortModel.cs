using Trivial.Domain.Geometry;
using Trivial.Domain.Models.Base;
using System.Collections.Generic;
using System.Numerics;

namespace Trivial.Domain.Models;

public class PortModel : Model, IHasBounds, IHasShape, ILinkable
{
    private readonly List<BaseLinkModel> m_Links = new(4);

    public PortModel(NodeModel Parent, Vector2 Position,
        Size Size, PortAlignment Alignment = PortAlignment.Bottom)
    {
        this.Parent = Parent;
        this.Alignment = Alignment;
        this.Position = Position;
        this.Size = Size;
    }

    public PortModel(string Id, NodeModel Parent, Vector2 Position, Size Size, PortAlignment Alignment = PortAlignment.Bottom) : base(Id)
    {
        this.Parent = Parent;
        this.Alignment = Alignment;
        this.Position = Position;
        this.Size = Size;
    }

    public NodeModel Parent { get; }
    public PortAlignment Alignment { get; }
    public Vector2 Position { get; set; }
    public Vector2 MiddlePosition => new(Position.X + (Size.Width / 2), Position.Y + (Size.Height / 2));
    public Size Size { get; set; }
    public IReadOnlyList<BaseLinkModel> Links => m_Links;
    /// <summary>
    /// If set to false, a call to Refresh() will force the port to update its position/size
    /// </summary>
    public bool Initialized { get; set; }

    public void RefreshAll()
    {
        Refresh();
        RefreshLinks();
    }

    public void RefreshLinks()
    {
        foreach (var t_Link in Links)
        {
            t_Link.Refresh();
            t_Link.RefreshLinks();
        }
    }

    public T GetParent<T>() where T : NodeModel => (T)Parent;

    public Rectangle GetBounds() => new(Position, Size);

    public virtual IShape GetShape() => Shapes.Circle(this);

    public virtual bool CanAttachTo(ILinkable Other)
    {
        // Todo: remove in order to support same node links
        return Other is PortModel t_Port && t_Port != this && !t_Port.Locked && Parent != t_Port.Parent;
    }

    void ILinkable.AddLink(BaseLinkModel Link) => m_Links.Add(Link);

    void ILinkable.RemoveLink(BaseLinkModel Link) => m_Links.Remove(Link);
}
