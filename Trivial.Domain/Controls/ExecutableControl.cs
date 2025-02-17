using Trivial.Graph.Domain.Events;
using Trivial.Graph.Domain.Models.Base;
using System.Threading.Tasks;

namespace Trivial.Graph.Domain.Controls;

public abstract class ExecutableControl : Control
{
    public abstract ValueTask OnPointerDown(Diagram Diagram, Model Model, PointerEventArgs E);
}