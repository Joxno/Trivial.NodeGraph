using System.Numerics;
using Trivial.Graph.Domain.Geometry;
using Trivial.Graph.Domain.Models.Base;

namespace Trivial.Graph.Domain.Positions;

public interface IPositionProvider
{
    public Vector2? GetPosition(Model Model);
}