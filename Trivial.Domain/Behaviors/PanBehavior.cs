using System.Numerics;
using Trivial.Graph.Domain.Models.Base;
using Trivial.Graph.Domain.Events;

namespace Trivial.Graph.Domain.Behaviors;

public class PanBehavior : BaseBehaviour
{
    private Maybe<Vector2> m_InitialPan = Maybe.None;
    private Vector2 m_LastClientPos;

    public MouseEventButton PanButton { get; set; } = MouseEventButton.Left;

    public PanBehavior(Diagram Diagram) : base(Diagram) {}

    protected override void _OnPointerDown(Maybe<Model> Model, PointerEventArgs E) => 
        FP.If(E.Button != (int)PanButton, () => Start(Model, E.GetClientPos(), E.ShiftKey));
    protected override void _OnPointerMove(Maybe<Model> Model, PointerEventArgs E) => Move(E.GetClientPos());
    protected override void _OnPointerUp(Maybe<Model> Model, PointerEventArgs E) => End();

    private void Start(Maybe<Model> Model, Vector2 Pos, bool ShiftKey)
    {
        if (!Diagram.Options.AllowPanning || Model.HasValue || ShiftKey) return;

        m_InitialPan = Diagram.Pan;
        m_LastClientPos = Pos;
    }

    private void Move(Vector2 Pos)
    {
        if (!Diagram.Options.AllowPanning || !m_InitialPan.HasValue) return;

        var t_Delta = Pos - m_LastClientPos - (Diagram.Pan - m_InitialPan.Value);
        Diagram.UpdatePan(t_Delta.X, t_Delta.Y);
    }

    private void End()
    {
        if (!Diagram.Options.AllowPanning) return;

        m_InitialPan = null!;
    }
}