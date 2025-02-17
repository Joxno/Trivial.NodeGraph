using System;
using System.Text;
using Trivial.Graph.Domain.Models.Base;
using Trivial.Graph.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Trivial.Graph.Components.Renderers;

public class LinkRenderer : ComponentBase, IDisposable
{
    private bool m_ShouldRender = true;

    [CascadingParameter] public BlazorDiagram BlazorDiagram { get; set; } = null!;

    [Parameter] public BaseLinkModel Link { get; set; } = null!;

    public void Dispose()
    {
        Link.Changed -= OnLinkChanged;
        Link.VisibilityChanged -= OnLinkChanged;
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Link.Changed += OnLinkChanged;
        Link.VisibilityChanged += OnLinkChanged;
    }

    protected override bool ShouldRender()
    {
        if (!m_ShouldRender)
            return false;

        m_ShouldRender = false;
        return true;
    }

    protected override void BuildRenderTree(RenderTreeBuilder Builder)
    {
        if (!Link.Visible)
            return;
        
        var t_ComponentType = BlazorDiagram.GetComponent(Link) ?? typeof(LinkWidget);
        var t_Classes = new StringBuilder()
            .Append("diagram-link")
            .AppendIf(" attached", Link.IsAttached)
            .ToString();

        Builder.OpenElement(0, "g");
        Builder.AddAttribute(1, "class", t_Classes);
        Builder.AddAttribute(2, "data-link-id", Link.Id);
        Builder.AddAttribute(3, "onpointerdown", EventCallback.Factory.Create<PointerEventArgs>(this, OnPointerDown));
        Builder.AddEventStopPropagationAttribute(4, "onpointerdown", true);
        Builder.AddAttribute(5, "onpointerup", EventCallback.Factory.Create<PointerEventArgs>(this, OnPointerUp));
        Builder.AddEventStopPropagationAttribute(6, "onpointerup", true);
        Builder.AddAttribute(7, "onmouseenter", EventCallback.Factory.Create<MouseEventArgs>(this, OnMouseEnter));
        Builder.AddAttribute(8, "onmouseleave", EventCallback.Factory.Create<MouseEventArgs>(this, OnMouseLeave));
        Builder.OpenComponent(9, t_ComponentType);
        Builder.AddAttribute(10, "Link", Link);
        Builder.CloseComponent();
        Builder.CloseElement();
    }

    private void OnLinkChanged(Model _)
    {
        m_ShouldRender = true;
        InvokeAsync(StateHasChanged);
    }

    private void OnPointerDown(PointerEventArgs E)
    {
        BlazorDiagram.TriggerPointerDown(Link, E.ToCore());
    }

    private void OnPointerUp(PointerEventArgs E)
    {
        BlazorDiagram.TriggerPointerUp(Link, E.ToCore());
    }

    private void OnMouseEnter(MouseEventArgs E)
    {
        BlazorDiagram.TriggerPointerEnter(Link, E.ToCore());
    }

    private void OnMouseLeave(MouseEventArgs E)
    {
        BlazorDiagram.TriggerPointerLeave(Link, E.ToCore());
    }
}