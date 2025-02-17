using Trivial.Graph.Domain.Models.Base;
using Trivial.Graph.Domain.Events;
using System.Diagnostics;

namespace Trivial.Graph.Domain.Behaviors;

public class EventsBehavior : BaseBehaviour
{
    private readonly Stopwatch m_MouseClickSw = new Stopwatch();
    private Model? m_Model;
    private bool m_CaptureMouseMove;
    private int m_MouseMovedCount;

    public EventsBehavior(Diagram Diagram) : base(Diagram) {}

    protected override void _OnPointerClick(Maybe<Model> Model, PointerEventArgs E)
    {
        if (m_MouseClickSw.IsRunning && m_MouseClickSw.ElapsedMilliseconds <= 500)
        {
            Diagram.TriggerPointerfloatClick(Model!.ValueOr(null), E);
        }

        m_MouseClickSw.Restart();
    }

    protected override void _OnPointerDown(Maybe<Model> Model, PointerEventArgs E)
    {
        m_CaptureMouseMove = true;
        m_MouseMovedCount = 0;
        m_Model = Model!.ValueOr(null);
    }

    protected override void _OnPointerMove(Maybe<Model> Model, PointerEventArgs E)
    {
        if (!m_CaptureMouseMove)
            return;

        m_MouseMovedCount++;
    }

    protected override void _OnPointerUp(Maybe<Model> Model, PointerEventArgs E)
    {
        if (!m_CaptureMouseMove) return; // Only set by OnMouseDown
        m_CaptureMouseMove = false;
        if (m_MouseMovedCount > 0) return;

        if (m_Model == Model)
        {
            Diagram.TriggerPointerClick(Model!.ValueOr(null), E);
            m_Model = null;
        }
    }

    protected override void _OnDispose() => m_Model = null;
}
