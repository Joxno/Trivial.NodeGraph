using System.Numerics;
using Trivial.Domain.Geometry;
using Trivial.Domain.Models.Base;

namespace Trivial.Domain.Positions;

public interface IPositionProvider
{
    public Vector2? GetPosition(Model model);
}