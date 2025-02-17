using Trivial.Domain.Geometry;
using Trivial.Domain.Models.Base;

namespace Trivial.Domain.Positions;

public interface IPositionProvider
{
    public Point? GetPosition(Model model);
}