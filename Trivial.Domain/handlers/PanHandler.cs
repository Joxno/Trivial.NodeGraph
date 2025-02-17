using System.Numerics;
using Trivial.Graph.Domain.Extensions;
using Trivial.Graph.Domain.Models.Base;

namespace Trivial.Graph.Domain.handlers;

public class PanHandler(Diagram Graph)
{
    public event Action? PanChanged;

    public void SetPan(float X, float Y)
    {
        Graph.Pan = new Vector2(X, Y);
        PanChanged?.Invoke();
        Graph.Refresh();
    }

    public void UpdatePan(float DeltaX, float DeltaY)
    {
        Graph.Pan += new Vector2(DeltaX, DeltaY);
        PanChanged?.Invoke();
        Graph.Refresh();
    }
}