using Trivial.Graph.Domain.Models.Base;
using Trivial.Graph.Domain.Models;
using Microsoft.AspNetCore.Components;
using System;
using Microsoft.AspNetCore.Components.Web;
using Trivial.Graph.Extensions;
using Trivial.Graph.Domain.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Trivial.Graph.Components.Renderers;

public class LinkVertexRenderer : ComponentBase, IDisposable
{
    private bool m_ShouldRender = true;

    [CascadingParameter] public BlazorDiagram BlazorDiagram { get; set; } = null!;
    [Parameter] public LinkVertexModel Vertex { get; set; } = null!;
    [Parameter] public string? Color { get; set; }
    [Parameter] public string? SelectedColor { get; set; }

    private string? ColorToUse => Vertex.Selected ? SelectedColor : Color;

    public void Dispose()
    {
        Vertex.Changed -= OnVertexChanged;
    }

    protected override void OnInitialized()
    {
        Vertex.Changed += OnVertexChanged;
    }

    protected override bool ShouldRender()
    {
        if (!m_ShouldRender) return false;

        m_ShouldRender = false;
        return true;
    }

    protected override void BuildRenderTree(RenderTreeBuilder Builder)
    {
        var t_ComponentType = BlazorDiagram.GetComponent(Vertex);

        Builder.OpenElement(0, "g");
        Builder.AddAttribute(1, "class", "diagram-link-vertex");
        Builder.AddAttribute(4, "cursor", "move");
        Builder.AddAttribute(5, "ondblclick", value: EventCallback.Factory.Create<MouseEventArgs>(this, OnfloatClick));
        Builder.AddAttribute(6, "onpointerdown", EventCallback.Factory.Create<PointerEventArgs>(this, OnPointerDown));
        Builder.AddAttribute(7, "onpointerup", EventCallback.Factory.Create<PointerEventArgs>(this, OnPointerUp));
        Builder.AddEventStopPropagationAttribute(8, "onpointerdown", true);
        Builder.AddEventStopPropagationAttribute(9, "onpointerup", true);

        if (t_ComponentType == null)
        {
            Builder.OpenElement(10, "circle");
            Builder.AddAttribute(11, "cx", Vertex.Position.X.ToInvariantString());
            Builder.AddAttribute(12, "cy", Vertex.Position.Y.ToInvariantString());
            Builder.AddAttribute(13, "r", "5");
            Builder.AddAttribute(14, "fill", ColorToUse);
            Builder.CloseElement();
        }
        else
        {
            Builder.OpenComponent(15, t_ComponentType);
            Builder.AddAttribute(16, "Vertex", Vertex);
            Builder.AddAttribute(17, "Color", ColorToUse);
            Builder.CloseComponent();
        }

        Builder.CloseElement();
    }

    private void OnVertexChanged(Model _)
    {
        m_ShouldRender = true;
        InvokeAsync(StateHasChanged);
    }

    private void OnPointerDown(PointerEventArgs E)
    {
        BlazorDiagram.TriggerPointerDown(Vertex, E.ToCore());
    }

    private void OnPointerUp(PointerEventArgs E)
    {
        BlazorDiagram.TriggerPointerUp(Vertex, E.ToCore());
    }

    private void OnfloatClick(MouseEventArgs E)
    {
        Vertex.Parent.Vertices.Remove(Vertex);
        Vertex.Parent.Refresh();
    }
}
