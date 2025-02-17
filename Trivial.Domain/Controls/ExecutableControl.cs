using Trivial.Domain.Events;
using Trivial.Domain.Models.Base;
using System.Threading.Tasks;

namespace Trivial.Domain.Controls;

public abstract class ExecutableControl : Control
{
    public abstract ValueTask OnPointerDown(Diagram Diagram, Model Model, PointerEventArgs E);
}