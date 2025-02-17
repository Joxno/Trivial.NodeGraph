using System;
using Trivial.Graph.Domain.Events;
using Trivial.Graph.Domain.Models.Base;
using Trivial.Functional;
using Trivial.Utilities;

namespace Trivial.Graph.Domain;

public abstract class BaseBehaviour : IDisposable
{
    protected Diagram Diagram { get; private set; }

    public BaseBehaviour(Diagram Diagram)
    {
        this.Diagram = Diagram;

        Diagram.PointerDown += (Model, E) => _OnPointerDown(Model!.ToMaybe(), E);
        Diagram.PointerMove += (Model, E) => _OnPointerMove(Model!.ToMaybe(), E);
        Diagram.PointerUp += (Model, E) => _OnPointerUp(Model!.ToMaybe(), E);
        Diagram.PointerEnter += (Model, E) => _OnPointerEnter(Model!.ToMaybe(), E);
        Diagram.PointerLeave += (Model, E) => _OnPointerLeave(Model!.ToMaybe(), E);
        Diagram.KeyDown += _OnKeyDown;
        Diagram.Wheel += _OnWheel;
        Diagram.PointerClick += (Model, E) => _OnPointerClick(Model!.ToMaybe(), E);
        Diagram.PointerDoubleClick += (Model, E) => _OnPointerDoubleClick(Model!.ToMaybe(), E);
    }

    protected virtual void _OnPointerDown(Maybe<Model> Model, PointerEventArgs E) {}
    protected virtual void _OnPointerMove(Maybe<Model> Model, PointerEventArgs E) {}
    protected virtual void _OnPointerUp(Maybe<Model> Model, PointerEventArgs E) {}
    protected virtual void _OnPointerEnter(Maybe<Model> Model, PointerEventArgs E) {}
    protected virtual void _OnPointerLeave(Maybe<Model> Model, PointerEventArgs E) {}
    protected virtual void _OnKeyDown(KeyboardEventArgs E) {}
    protected virtual void _OnWheel(WheelEventArgs E) {}
    protected virtual void _OnPointerClick(Maybe<Model> Model, PointerEventArgs E) {}
    protected virtual void _OnPointerDoubleClick(Maybe<Model> Model, PointerEventArgs E) {}

    public void Dispose()
    {
        Diagram.PointerDown -= (Model, E) => _OnPointerDown(Model!.ToMaybe(), E);
        Diagram.PointerMove -= (Model, E) => _OnPointerMove(Model!.ToMaybe(), E);
        Diagram.PointerUp -= (Model, E) => _OnPointerUp(Model!.ToMaybe(), E);
        Diagram.PointerEnter -= (Model, E) => _OnPointerEnter(Model!.ToMaybe(), E);
        Diagram.PointerLeave -= (Model, E) => _OnPointerLeave(Model!.ToMaybe(), E);
        Diagram.KeyDown -= _OnKeyDown;
        Diagram.Wheel -= _OnWheel;
        Diagram.PointerClick -= (Model, E) => _OnPointerClick(Model!.ToMaybe(), E);
        Diagram.PointerDoubleClick -= (Model, E) => _OnPointerDoubleClick(Model!.ToMaybe(), E);
        _OnDispose();
    }
    protected virtual void _OnDispose() {}
}
