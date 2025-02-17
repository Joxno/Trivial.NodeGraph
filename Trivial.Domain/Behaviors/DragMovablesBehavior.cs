using Trivial.Graph.Domain.Models.Base;
using Trivial.Graph.Domain.Events;
using System.Numerics;
using Trivial.Graph.Domain.Models;
using Trivial.Functional;
using Trivial.Graph.Domain.Extensions;

namespace Trivial.Graph.Domain.Behaviors;

public class DragMovablesBehavior : BaseBehaviour
{
    private readonly List<InitialPosition> m_InitialPositions = new();
    private Vector2 m_LastClientPos;
    public bool IsMoving { get; private set; } = false;

    public DragMovablesBehavior(Diagram Diagram) : base(Diagram) {}

    protected override void _OnPointerDown(Maybe<Model> Model, PointerEventArgs E)
    {
        if (!Model.HasValue) return;
        if (Model.Value is not MovableModel)
            return;

        m_InitialPositions.Clear();
        foreach (var t_Sm in Diagram.GetSelectedModels())
        {
            if (t_Sm is not MovableModel t_Movable || t_Movable.Locked) continue;
            if (t_Sm is NodeModel t_Node && t_Node.Group != null && !t_Node.Group.AutoSize) continue; // Special case: groups without auto size on

            var t_Position = t_Movable.Position;
            if (Diagram.Options.GridSnapToCenter && t_Movable is NodeModel t_N)
            {
                t_Position = new Vector2(
                    t_Movable.Position.X + (t_N.Size.Width) / 2,
                    t_Movable.Position.Y + (t_N.Size.Height) / 2);
            }

            m_InitialPositions.Add(new(t_Movable, t_Position));
        }

        m_LastClientPos = new Vector2(E.ClientX, E.ClientY);
        IsMoving = false;
    }

    protected override void _OnPointerMove(Maybe<Model> Model, PointerEventArgs E)
    {
        if (m_InitialPositions.Count == 0 || m_LastClientPos.DistanceTo(E.GetClientPos()) < 0.01f)
            return;

        IsMoving = true;
        var t_Delta = (E.GetClientPos() - m_LastClientPos) / Diagram.Zoom;
        foreach (var (t_Movable, t_InitialPosition) in m_InitialPositions)
        {
            var t_Ndx = ApplyGridSize(t_Delta.X + t_InitialPosition.X);
            var t_Ndy = ApplyGridSize(t_Delta.Y + t_InitialPosition.Y);
            if (Diagram.Options.GridSnapToCenter && t_Movable is NodeModel t_Node)
            {
                t_Node.SetPosition(t_Ndx - (t_Node.Size.Width) / 2, t_Ndy - (t_Node.Size.Height) / 2);
            }
            else
            {
                t_Movable.SetPosition(t_Ndx, t_Ndy);
            }
        }
    }

    protected override void _OnPointerUp(Maybe<Model> Model, PointerEventArgs E)
    {
        if (m_InitialPositions.Count == 0) return;

        if (IsMoving)
        {
            foreach (var (t_Movable, _) in m_InitialPositions)
            {
                t_Movable.TriggerMoved();
            }
        }
        
        m_InitialPositions.Clear();
        IsMoving = false;
    }

    private float ApplyGridSize(float N)
    {
        if (Diagram.Options.GridSize == null)
            return N;

        var t_GridSize = Diagram.Options.GridSize.Value;
        return t_GridSize * MathF.Floor((N + t_GridSize / 2.0f) / t_GridSize);
    }

    protected override void _OnDispose() => m_InitialPositions.Clear();

    private record struct InitialPosition(MovableModel Model, Vector2 Pos);
}