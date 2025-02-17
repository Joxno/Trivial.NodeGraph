using Trivial.Graph.Domain.Models;
using Trivial.Graph.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Trivial.Graph.Components;

public partial class LinkWidget
{
    private bool m_Hovered;

    [CascadingParameter] public BlazorDiagram BlazorDiagram { get; set; } = null!;
    [Parameter] public LinkModel Link { get; set; } = null!;

    private RenderFragment GetSelectionHelperPath(string Color, string D, int Index)
    {
        return Builder =>
        {
            Builder.OpenElement(0, "path");
            Builder.AddAttribute(1, "class", "selection-helper");
            Builder.AddAttribute(2, "stroke", Color);
            Builder.AddAttribute(3, "stroke-width", 12);
            Builder.AddAttribute(4, "d", D);
            Builder.AddAttribute(5, "stroke-linecap", "butt");
            Builder.AddAttribute(6, "stroke-opacity", m_Hovered ? "0.05" : "0");
            Builder.AddAttribute(7, "fill", "none");
            Builder.AddAttribute(8, "onmouseenter", EventCallback.Factory.Create<MouseEventArgs>(this, OnMouseEnter));
            Builder.AddAttribute(9, "onmouseleave", EventCallback.Factory.Create<MouseEventArgs>(this, OnMouseLeave));
            Builder.AddAttribute(10, "onpointerdown", EventCallback.Factory.Create<Microsoft.AspNetCore.Components.Web.PointerEventArgs>(this, E => OnPointerDown(E, Index)));
            Builder.AddEventStopPropagationAttribute(11, "onpointerdown", Link.Segmentable);
            Builder.CloseElement();
        };
    }

    private void OnPointerDown(PointerEventArgs E, int Index)
    {
        if (!Link.Segmentable)
            return;

        var t_Vertex = CreateVertex((float)E.ClientX, (float)E.ClientY, Index);
        BlazorDiagram.TriggerPointerDown(t_Vertex, E.ToCore());
    }

    private void OnMouseEnter(MouseEventArgs E)
    {
        m_Hovered = true;
    }

    private void OnMouseLeave(MouseEventArgs E)
    {
        m_Hovered = false;
    }

    private LinkVertexModel CreateVertex(float ClientX, float ClientY, int Index)
    {
        var t_RPt = BlazorDiagram.GetRelativeMousePoint(ClientX, ClientY);
        var t_Vertex = new LinkVertexModel(Link, t_RPt);
        Link.Vertices.Insert(Index, t_Vertex);
        Link.Refresh();
        return t_Vertex;
    }
}
