using System;
using System.Numerics;
using Trivial.Graph.Domain.Extensions;
using Trivial.Graph.Domain.Geometry;
using Trivial.Graph.Domain.Models;
using Trivial.Graph.Domain.Models.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using SvgPathProperties;

namespace Trivial.Graph.Components.Renderers;

public class LinkLabelRenderer : ComponentBase, IDisposable
{
    [CascadingParameter] public BlazorDiagram BlazorDiagram { get; set; } = null!;
    [Parameter] public LinkLabelModel Label { get; set; } = null!;
    [Parameter] public SvgPath Path { get; set; } = null!;

    public void Dispose()
    {
        Label.Changed -= OnLabelChanged;
        Label.VisibilityChanged -= OnLabelChanged;
    }

    protected override void OnInitialized()
    {
        Label.Changed += OnLabelChanged;
        Label.VisibilityChanged += OnLabelChanged;
    }

    protected override void BuildRenderTree(RenderTreeBuilder Builder)
    {
        if (!Label.Visible)
            return;
        
        var t_Position = FindPosition();
        if (t_Position == null)
            return;
        
        var t_ComponentType = BlazorDiagram.GetComponent(Label) ?? typeof(DefaultLinkLabelWidget);

        Builder.OpenElement(0, "foreignObject");
        Builder.AddAttribute(1, "class", "diagram-link-label");
        Builder.AddAttribute(2, "x", (t_Position.Value.X + (Label.Offset?.X ?? 0)).ToInvariantString());
        Builder.AddAttribute(3, "y", (t_Position.Value.Y + (Label.Offset?.Y ?? 0)).ToInvariantString());
        
        Builder.OpenComponent(4, t_ComponentType);
        Builder.AddAttribute(5, "Label", Label);
        Builder.CloseComponent();
        
        Builder.CloseElement();
    }

    private void OnLabelChanged(Model _)
    {
        InvokeAsync(StateHasChanged);
    }

    private Vector2? FindPosition()
    {
        var t_TotalLength = Path.Length;
        var t_Length = Label.Distance switch
        {
            <= 1 and >= 0 => Label.Distance.Value * t_TotalLength,
            > 1 => Label.Distance.Value,
            < 0 => t_TotalLength + Label.Distance.Value,
            _ => t_TotalLength * (Label.Parent.Labels.IndexOf(Label) + 1) / (Label.Parent.Labels.Count + 1)
        };

        var t_Pt = Path.GetPointAtLength(t_Length);
        return new Vector2((float)t_Pt.X, (float)t_Pt.Y);
    }
}