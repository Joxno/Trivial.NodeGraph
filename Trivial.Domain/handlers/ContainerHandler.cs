using System.Numerics;
using Trivial.Graph.Domain.Geometry;
using Trivial.Graph.Domain.Models.Base;

namespace Trivial.Graph.Domain.handlers;

public class ContainerHandler(Diagram Graph)
{
    public event Action? ContainerChanged;
    
    public void SetContainer(Rectangle NewRect)
    {
        if (NewRect.Equals(Graph.Container))
            return;

        Graph.Container = NewRect;
        ContainerChanged?.Invoke();
        Graph.Refresh();
    }

    public Vector2 GetRelativeMousePoint(float ClientX, float ClientY)
    {
        if (Graph.Container == null)
            throw new Exception(
                "Container not available. Make sure you're not using this method before the diagram is fully loaded");

        return new Vector2((ClientX - Graph.Container.Left - Graph.Pan.X) / Graph.Zoom, (ClientY - Graph.Container.Top - Graph.Pan.Y) / Graph.Zoom);
    }

    public Vector2 GetRelativePoint(float ClientX, float ClientY)
    {
        if (Graph.Container == null)
            throw new Exception(
                "Container not available. Make sure you're not using this method before the diagram is fully loaded");

        return new Vector2(ClientX - Graph.Container.Left, ClientY - Graph.Container.Top);
    }

    public Vector2 GetScreenPoint(float ClientX, float ClientY)
    {
        if (Graph.Container == null)
            throw new Exception(
                "Container not available. Make sure you're not using this method before the diagram is fully loaded");

        return new Vector2(Graph.Zoom * ClientX + Graph.Container.Left + Graph.Pan.X, Graph.Zoom * ClientY + Graph.Container.Top + Graph.Pan.Y);
    }
}