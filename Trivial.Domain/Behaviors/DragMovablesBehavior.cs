using Trivial.Domain.Geometry;
using Trivial.Domain.Models.Base;
using Trivial.Domain.Events;
using System;
using System.Collections.Generic;
using System.Numerics;
using Trivial.Domain.Models;

namespace Trivial.Domain.Behaviors;

public class DragMovablesBehavior : Behavior
{
    private readonly Dictionary<MovableModel, Vector2> m_InitialPositions;
    private float? m_LastClientX;
    private float? m_LastClientY;
    private bool m_Moved;

    public DragMovablesBehavior(Diagram Diagram) : base(Diagram)
    {
        m_InitialPositions = new Dictionary<MovableModel, Vector2>();
        base.Diagram.PointerDown += OnPointerDown;
        base.Diagram.PointerMove += OnPointerMove;
        base.Diagram.PointerUp += OnPointerUp;
    }

    private void OnPointerDown(Model? Model, PointerEventArgs E)
    {
        if (Model is not MovableModel)
            return;

        m_InitialPositions.Clear();
        foreach (var t_Sm in Diagram.GetSelectedModels())
        {
            if (t_Sm is not MovableModel t_Movable || t_Movable.Locked)
                continue;

            // Special case: groups without auto size on
            if (t_Sm is NodeModel t_Node && t_Node.Group != null && !t_Node.Group.AutoSize)
                continue;

            var t_Position = t_Movable.Position;
            if (Diagram.Options.GridSnapToCenter && t_Movable is NodeModel t_N)
            {
                t_Position = new Vector2(t_Movable.Position.X + (t_N.Size.Width) / 2,
                    t_Movable.Position.Y + (t_N.Size.Height) / 2);
            }

            m_InitialPositions.Add(t_Movable, t_Position);
        }

        m_LastClientX = E.ClientX;
        m_LastClientY = E.ClientY;
        m_Moved = false;
    }

    private void OnPointerMove(Model? Model, PointerEventArgs E)
    {
        if (m_InitialPositions.Count == 0 || m_LastClientX == null || m_LastClientY == null)
            return;

        m_Moved = true;
        var t_DeltaX = (E.ClientX - m_LastClientX.Value) / Diagram.Zoom;
        var t_DeltaY = (E.ClientY - m_LastClientY.Value) / Diagram.Zoom;

        foreach (var (t_Movable, t_InitialPosition) in m_InitialPositions)
        {
            var t_Ndx = ApplyGridSize(t_DeltaX + t_InitialPosition.X);
            var t_Ndy = ApplyGridSize(t_DeltaY + t_InitialPosition.Y);
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

    private void OnPointerUp(Model? Model, PointerEventArgs E)
    {
        if (m_InitialPositions.Count == 0)
            return;

        if (m_Moved)
        {
            foreach (var (t_Movable, _) in m_InitialPositions)
            {
                t_Movable.TriggerMoved();
            }
        }
        
        m_InitialPositions.Clear();
        m_LastClientX = null;
        m_LastClientY = null;
    }

    private float ApplyGridSize(float N)
    {
        if (Diagram.Options.GridSize == null)
            return N;

        var t_GridSize = Diagram.Options.GridSize.Value;
        return t_GridSize * MathF.Floor((N + t_GridSize / 2.0f) / t_GridSize);
    }

    public override void Dispose()
    {
        m_InitialPositions.Clear();
        
        Diagram.PointerDown -= OnPointerDown;
        Diagram.PointerMove -= OnPointerMove;
        Diagram.PointerUp -= OnPointerUp;
    }
}
