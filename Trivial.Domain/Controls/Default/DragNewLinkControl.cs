using System.Numerics;
using System.Threading.Tasks;
using Trivial.Domain.Behaviors;
using Trivial.Domain.Events;
using Trivial.Domain.Geometry;
using Trivial.Domain.Models;
using Trivial.Domain.Models.Base;
using Trivial.Domain.Positions;

namespace Trivial.Domain.Controls.Default;

public class DragNewLinkControl : ExecutableControl
{
    private readonly IPositionProvider m_PositionProvider;

    public DragNewLinkControl(float X, float Y, float OffsetX = 0, float OffsetY = 0)
        : this(new BoundsBasedPositionProvider(X, Y, OffsetX, OffsetY))
    {
    }

    public DragNewLinkControl(IPositionProvider PositionProvider)
    {
        m_PositionProvider = PositionProvider;
    }

    public override Vector2? GetPosition(Model Model) => m_PositionProvider.GetPosition(Model);

    public override ValueTask OnPointerDown(Diagram Diagram, Model Model, PointerEventArgs E)
    {
        if (Model is not NodeModel t_Node || t_Node.Locked)
            return ValueTask.CompletedTask;
        
        var t_Behavior = Diagram.GetBehavior<DragNewLinkBehavior>();
        if (t_Behavior == null)
            throw new DiagramsException($"DragNewLinkBehavior was not found");

        t_Behavior.StartFrom(t_Node, E.ClientX, E.ClientY);
        return ValueTask.CompletedTask;
    }
}