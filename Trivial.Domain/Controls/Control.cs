using Trivial.Domain.Geometry;
using Trivial.Domain.Models.Base;

namespace Trivial.Domain.Controls;

public abstract class Control
{
    public abstract Point? GetPosition(Model model);
}