using System;
using System.Linq;
using Trivial.Domain.Extensions;
using Trivial.Domain.Geometry;
using Trivial.Domain.Models;
using Trivial.Domain.Models.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Trivial.Graph.Components.Widgets;

public partial class NavigatorWidget : IDisposable
{
    private float m_X;
    private float m_Y;
    private float m_Width;
    private float m_Height;
    private float m_ScaledMargin;
    private float m_VX;
    private float m_VY;
    private float m_VWidth;
    private float m_VHeight;

    [CascadingParameter] public BlazorDiagram BlazorDiagram { get; set; } = null!;
    [Parameter] public bool UseNodeShape { get; set; } = true;
    [Parameter] public float Width { get; set; }
    [Parameter] public float Height { get; set; }
    [Parameter] public float Margin { get; set; } = 5;
    [Parameter] public string NodeColor { get; set; } = "#40babd";
    [Parameter] public string GroupColor { get; set; } = "#9fd0d1";
    [Parameter] public string ViewStrokeColor { get; set; } = "#40babd";
    [Parameter] public int ViewStrokeWidth { get; set; } = 4;
    [Parameter] public string? Class { get; set; }
    [Parameter] public string? Style { get; set; }

    public void Dispose()
    {
        BlazorDiagram.Changed -= Refresh;
        BlazorDiagram.Nodes.Added -= OnNodeAdded;
        BlazorDiagram.Nodes.Removed -= OnNodeRemoved;
        BlazorDiagram.Groups.Added -= OnNodeAdded;
        BlazorDiagram.Groups.Removed -= OnNodeRemoved;

        foreach (var t_Node in BlazorDiagram.Nodes)
            t_Node.Changed -= OnNodeChanged;

        foreach (var t_Group in BlazorDiagram.Groups)
            t_Group.Changed -= OnNodeChanged;
    }

    protected override void OnInitialized()
    {
        BlazorDiagram.Changed += Refresh;
        BlazorDiagram.Nodes.Added += OnNodeAdded;
        BlazorDiagram.Nodes.Removed += OnNodeRemoved;
        BlazorDiagram.Groups.Added += OnNodeAdded;
        BlazorDiagram.Groups.Removed += OnNodeRemoved;

        foreach (var t_Node in BlazorDiagram.Nodes)
            t_Node.Changed += OnNodeChanged;

        foreach (var t_Group in BlazorDiagram.Groups)
            t_Group.Changed += OnNodeChanged;
    }

    private void OnNodeAdded(NodeModel Node) => Node.Changed += OnNodeChanged;

    private void OnNodeRemoved(NodeModel Node) => Node.Changed -= OnNodeChanged;

    private void OnNodeChanged(Model _) => Refresh();

    private void Refresh()
    {
        if (BlazorDiagram.Container == null)
            return;

        m_VX = -BlazorDiagram.Pan.X / BlazorDiagram.Zoom;
        m_VY = -BlazorDiagram.Pan.Y / BlazorDiagram.Zoom;
        m_VWidth = BlazorDiagram.Container.Width / BlazorDiagram.Zoom;
        m_VHeight = BlazorDiagram.Container.Height / BlazorDiagram.Zoom;

        var t_MinX = m_VX;
        var t_MinY = m_VY;
        var t_MaxX = m_VX + m_VWidth;
        var t_MaxY = m_VY + m_VHeight;

        foreach (var t_Node in BlazorDiagram.Nodes.Union(BlazorDiagram.Groups))
        {
            if (t_Node.Size == null)
                continue;

            t_MinX = MathF.Min(t_MinX, t_Node.Position.X);
            t_MinY = MathF.Min(t_MinY, t_Node.Position.Y);
            t_MaxX = MathF.Max(t_MaxX, t_Node.Position.X + t_Node.Size.Width);
            t_MaxY = MathF.Max(t_MaxY, t_Node.Position.Y + t_Node.Size.Height);
        }

        var t_Width = t_MaxX - t_MinX;
        var t_Height = t_MaxY - t_MinY;
        var t_ScaledWidth = t_Width / Width;
        var t_ScaledHeight = t_Height / Height;
        var t_Scale = MathF.Max(t_ScaledWidth, t_ScaledHeight);
        var t_ViewWidth = t_Scale * Width;
        var t_ViewHeight = t_Scale * Height;

        m_ScaledMargin = Margin * t_Scale;
        m_X = t_MinX - (t_ViewWidth - t_Width) / 2 - m_ScaledMargin;
        m_Y = t_MinY - (t_ViewHeight - t_Height) / 2 - m_ScaledMargin;
        m_Width = t_ViewWidth + m_ScaledMargin * 2;
        m_Height = t_ViewHeight + m_ScaledMargin * 2;
        InvokeAsync(StateHasChanged);
    }

    private RenderFragment GetNodeRenderFragment(NodeModel Node)
    {
        return Builder =>
        {
            if (UseNodeShape)
            {
                var t_Shape = Node.GetShape();
                if (t_Shape is Ellipse t_Ellipse)
                {
                    RenderEllipse(Node, Builder, t_Ellipse);
                    return;
                }
            }
            
            RenderRect(Node, Builder);
        };
    }

    private void RenderRect(NodeModel Node, RenderTreeBuilder Builder)
    {
        Builder.OpenElement(0, "rect");
        Builder.SetKey(Node);
        Builder.AddAttribute(1, "class", "navigator-node");
        Builder.AddAttribute(2, "fill", NodeColor);
        Builder.AddAttribute(2, "x", Node.Position.X.ToInvariantString());
        Builder.AddAttribute(2, "y", Node.Position.Y.ToInvariantString());
        Builder.AddAttribute(2, "width", Node.Size!.Width.ToInvariantString());
        Builder.AddAttribute(2, "height", Node.Size.Height.ToInvariantString());
        Builder.CloseElement();
    }

    private void RenderEllipse(NodeModel Node, RenderTreeBuilder Builder, Ellipse Ellipse)
    {
        Builder.OpenElement(0, "ellipse");
        Builder.SetKey(Node);
        Builder.AddAttribute(1, "class", "navigator-node");
        Builder.AddAttribute(2, "fill", NodeColor);
        Builder.AddAttribute(2, "cx", Ellipse.Cx.ToInvariantString());
        Builder.AddAttribute(2, "cy", Ellipse.Cy.ToInvariantString());
        Builder.AddAttribute(2, "rx",Ellipse.Rx.ToInvariantString());
        Builder.AddAttribute(2, "ry", Ellipse.Ry.ToInvariantString());
        Builder.CloseElement();
    }
}