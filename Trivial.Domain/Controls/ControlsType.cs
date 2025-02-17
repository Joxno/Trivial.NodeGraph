namespace Trivial.Graph.Domain.Controls;

[Flags]
public enum ControlsType
{
    None = 0,
    OnHover = 1,
    OnSelection = 2,
    OnDoubleClick = 4
}