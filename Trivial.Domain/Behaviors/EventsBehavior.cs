using Trivial.Domain.Models.Base;
using Trivial.Domain.Events;
using System.Diagnostics;

namespace Trivial.Domain.Behaviors;

public class EventsBehavior : Behavior
{
    private readonly Stopwatch m_MouseClickSw;
    private Model? m_Model;
    private bool m_CaptureMouseMove;
    private int m_MouseMovedCount;

    public EventsBehavior(Diagram Diagram) : base(Diagram)
    {
        m_MouseClickSw = new Stopwatch();

        base.Diagram.PointerDown += OnPointerDown;
        base.Diagram.PointerMove += OnPointerMove;
        base.Diagram.PointerUp += OnPointerUp;
        base.Diagram.PointerClick += OnPointerClick;
    }

    private void OnPointerClick(Model? Model, PointerEventArgs E)
    {
        if (m_MouseClickSw.IsRunning && m_MouseClickSw.ElapsedMilliseconds <= 500)
        {
            Diagram.TriggerPointerfloatClick(Model, E);
        }

        m_MouseClickSw.Restart();
    }

    private void OnPointerDown(Model? Model, PointerEventArgs E)
    {
        m_CaptureMouseMove = true;
        m_MouseMovedCount = 0;
        m_Model = Model;
    }

    private void OnPointerMove(Model? Model, PointerEventArgs E)
    {
        if (!m_CaptureMouseMove)
            return;

        m_MouseMovedCount++;
    }

    private void OnPointerUp(Model? Model, PointerEventArgs E)
    {
        if (!m_CaptureMouseMove) return; // Only set by OnMouseDown
        m_CaptureMouseMove = false;
        if (m_MouseMovedCount > 0) return;

        if (m_Model == Model)
        {
            Diagram.TriggerPointerClick(Model, E);
            m_Model = null;
        }
    }

    public override void Dispose()
    {
        Diagram.PointerDown -= OnPointerDown;
        Diagram.PointerMove -= OnPointerMove;
        Diagram.PointerUp -= OnPointerUp;
        Diagram.PointerClick -= OnPointerClick;
        m_Model = null;
    }
}
