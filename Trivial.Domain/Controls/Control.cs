using System.Numerics;
using Trivial.Graph.Domain.Geometry;
using Trivial.Graph.Domain.Models.Base;

namespace Trivial.Graph.Domain.Controls;

public abstract class Control
{
    public abstract Vector2? GetPosition(Model Model);
}