using System.Numerics;
using System.Threading.Tasks;
using Trivial.Graph.Domain.Behaviors;
using Trivial.Graph.Domain.Events;
using Trivial.Graph.Domain.Geometry;
using Trivial.Graph.Domain.Models;
using Trivial.Graph.Domain.Models.Base;
using Trivial.Graph.Domain.Positions;

namespace Trivial.Graph.Domain.Controls.Default;

public class DragNewLinkControl : ExecutableControl
{
    private readonly IPositionProvider m_PositionProvider;

    public DragNewLinkControl(float X, float Y, float OffsetX = 0, float OffsetY = 0)
        : this(new BoundsBasedPositionProvider(X, Y, OffsetX, OffsetY)) {}
    public DragNewLinkControl(IPositionProvider PositionProvider) => m_PositionProvider = PositionProvider;

    public override Vector2? GetPosition(Model Model) => m_PositionProvider.GetPosition(Model);

    public override ValueTask OnPointerDown(Diagram Diagram, Model Model, PointerEventArgs E)
    {
        if (Model is not NodeModel t_Node || t_Node.Locked)
            return ValueTask.CompletedTask;
        
        var t_Behavior = Diagram.GetBehavior<DragNewLinkBehavior>();
        if (t_Behavior == null)
            throw new DiagramsException($"DragNewLinkBehavior was not found");

        t_Behavior.StartFrom(t_Node, E.GetClientPos());
        return ValueTask.CompletedTask;
    }
}