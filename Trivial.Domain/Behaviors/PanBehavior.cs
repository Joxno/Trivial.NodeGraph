﻿using System.Numerics;
using Trivial.Domain.Geometry;
using Trivial.Domain.Models.Base;
using Trivial.Domain.Events;

namespace Trivial.Domain.Behaviors;

public class PanBehavior : Behavior
{
    private Vector2? _initialPan;
    private float _lastClientX;
    private float _lastClientY;

    public PanBehavior(Diagram diagram) : base(diagram)
    {
        Diagram.PointerDown += OnPointerDown;
        Diagram.PointerMove += OnPointerMove;
        Diagram.PointerUp += OnPointerUp;
    }

    private void OnPointerDown(Model? model, PointerEventArgs e)
    {
        if (e.Button != (int)MouseEventButton.Left)
            return;

        Start(model, e.ClientX, e.ClientY, e.ShiftKey);
    }

    private void OnPointerMove(Model? model, PointerEventArgs e) => Move(e.ClientX, e.ClientY);

    private void OnPointerUp(Model? model, PointerEventArgs e) => End();

    private void Start(Model? model, float clientX, float clientY, bool shiftKey)
    {
        if (!Diagram.Options.AllowPanning || model != null || shiftKey)
            return;

        _initialPan = Diagram.Pan;
        _lastClientX = clientX;
        _lastClientY = clientY;
    }

    private void Move(float clientX, float clientY)
    {
        if (!Diagram.Options.AllowPanning || _initialPan == null)
            return;

        var deltaX = clientX - _lastClientX - (Diagram.Pan.X - _initialPan.Value.X);
        var deltaY = clientY - _lastClientY - (Diagram.Pan.Y - _initialPan.Value.Y);
        Diagram.UpdatePan(deltaX, deltaY);
    }

    private void End()
    {
        if (!Diagram.Options.AllowPanning)
            return;

        _initialPan = null;
    }

    public override void Dispose()
    {
        Diagram.PointerDown -= OnPointerDown;
        Diagram.PointerMove -= OnPointerMove;
        Diagram.PointerUp -= OnPointerUp;
    }
}
