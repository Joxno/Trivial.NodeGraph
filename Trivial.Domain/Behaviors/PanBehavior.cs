using System.Numerics;
using Trivial.Domain.Geometry;
using Trivial.Domain.Models.Base;
using Trivial.Domain.Events;

namespace Trivial.Domain.Behaviors;

public class PanBehavior : Behavior
{
    private Vector2? m_InitialPan;
    private float m_LastClientX;
    private float m_LastClientY;

    public PanBehavior(Diagram Diagram) : base(Diagram)
    {
        base.Diagram.PointerDown += OnPointerDown;
        base.Diagram.PointerMove += OnPointerMove;
        base.Diagram.PointerUp += OnPointerUp;
    }

    private void OnPointerDown(Model? Model, PointerEventArgs E)
    {
        if (E.Button != (int)MouseEventButton.Left)
            return;

        Start(Model, E.ClientX, E.ClientY, E.ShiftKey);
    }

    private void OnPointerMove(Model? Model, PointerEventArgs E) => Move(E.ClientX, E.ClientY);

    private void OnPointerUp(Model? Model, PointerEventArgs E) => End();

    private void Start(Model? Model, float ClientX, float ClientY, bool ShiftKey)
    {
        if (!Diagram.Options.AllowPanning || Model != null || ShiftKey)
            return;

        m_InitialPan = Diagram.Pan;
        m_LastClientX = ClientX;
        m_LastClientY = ClientY;
    }

    private void Move(float ClientX, float ClientY)
    {
        if (!Diagram.Options.AllowPanning || m_InitialPan == null)
            return;

        var t_DeltaX = ClientX - m_LastClientX - (Diagram.Pan.X - m_InitialPan.Value.X);
        var t_DeltaY = ClientY - m_LastClientY - (Diagram.Pan.Y - m_InitialPan.Value.Y);
        Diagram.UpdatePan(t_DeltaX, t_DeltaY);
    }

    private void End()
    {
        if (!Diagram.Options.AllowPanning)
            return;

        m_InitialPan = null;
    }

    public override void Dispose()
    {
        Diagram.PointerDown -= OnPointerDown;
        Diagram.PointerMove -= OnPointerMove;
        Diagram.PointerUp -= OnPointerUp;
    }
}
