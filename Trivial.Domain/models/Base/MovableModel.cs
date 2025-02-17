using System;
using System.Numerics;
using Trivial.Domain.Geometry;

namespace Trivial.Domain.Models.Base;

// I'm assuming that all movable models (nodes & groups for now) are also selectable,
// I believe it makes sense since if you click to move something then you're also selecting
public abstract class MovableModel : SelectableModel
{
    public event Action<MovableModel>? Moved;
    
    public MovableModel(Vector2? Position = null)
    {
        this.Position = Position ?? Vector2.Zero;
    }

    public MovableModel(string Id, Vector2? Position = null) : base(Id)
    {
        this.Position = Position ?? Vector2.Zero;
    }

    public Vector2 Position { get; set; }

    public virtual void SetPosition(float X, float Y) => Position = new Vector2(X, Y);

    /// <summary>
    /// Only use this if you know what you're doing
    /// </summary>
    public void TriggerMoved() => Moved?.Invoke(this);
}
