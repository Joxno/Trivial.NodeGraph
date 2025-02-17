using System;
using System.Text;
using Trivial.Domain.Geometry;
using Trivial.Domain.Models;
using Trivial.Domain.Models.Base;
using Trivial.Graph.Extensions;
using Trivial.Graph.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Trivial.Graph.Components.Renderers;

public class GroupRenderer : ComponentBase, IDisposable
{
    private bool m_IsSvg;
    private Size? m_LastSize;
    private bool m_ShouldRender = true;

    [CascadingParameter] public BlazorDiagram BlazorDiagram { get; set; } = null!;

    [Parameter] public GroupModel Group { get; set; } = null!;

    public void Dispose()
    {
        Group.Changed -= OnGroupChanged;
        Group.VisibilityChanged -= OnGroupChanged;
    }

    protected override void OnInitialized()
    {
        Group.Changed += OnGroupChanged;
        Group.VisibilityChanged += OnGroupChanged;
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        m_IsSvg = Group is SvgGroupModel;
    }

    protected override bool ShouldRender()
    {
        if (m_ShouldRender)
        {
            m_ShouldRender = false;
            return true;
        }

        return false;
    }

    protected override void OnAfterRender(bool FirstRender)
    {
        if (!Group.Size.IsVisibleSize()) return;

        // Update the port positions (and links) when the size of the group changes
        // This will save us some JS trips as well as useless rerenders

        if (m_LastSize == null || !m_LastSize.Equals(Group.Size))
        {
            Group.ReinitializePorts();
            Group.RefreshLinks();
            m_LastSize = Group.Size;
        }
    }

    private void OnGroupChanged(Model _)
    {
        m_ShouldRender = true;
        InvokeAsync(StateHasChanged);
    }

    private static string GenerateStyle(float Top, float Left, float Width, float Height)
    {
        return FormattableString.Invariant($"top: {Top}px; left: {Left}px; width: {Width}px; height: {Height}px");
    }

    protected override void BuildRenderTree(RenderTreeBuilder Builder)
    {
        if (!Group.Visible)
            return;
        
        var t_ComponentType = BlazorDiagram.GetComponent(Group) ?? typeof(DefaultGroupWidget);
        var t_Classes = new StringBuilder("diagram-group")
            .AppendIf(" locked", Group.Locked)
            .AppendIf(" selected", Group.Selected)
            .AppendIf(" default", t_ComponentType == typeof(DefaultGroupWidget));

        Builder.OpenElement(0, m_IsSvg ? "g" : "div");
        Builder.AddAttribute(1, "class", t_Classes.ToString());
        Builder.AddAttribute(2, "data-group-id", Group.Id);

        if (m_IsSvg)
            Builder.AddAttribute(3, "transform",
                FormattableString.Invariant($"translate({Group.Position.X} {Group.Position.Y})"));
        else
            Builder.AddAttribute(3, "style",
                GenerateStyle(Group.Position.Y, Group.Position.X, Group.Size!.Width, Group.Size.Height));

        Builder.AddAttribute(4, "onpointerdown", EventCallback.Factory.Create<PointerEventArgs>(this, OnPointerDown));
        Builder.AddEventStopPropagationAttribute(5, "onpointerdown", true);
        Builder.AddAttribute(6, "onpointerup", EventCallback.Factory.Create<PointerEventArgs>(this, OnPointerUp));
        Builder.AddEventStopPropagationAttribute(7, "onpointerup", true);
        Builder.AddAttribute(8, "onmouseenter", EventCallback.Factory.Create<MouseEventArgs>(this, OnMouseEnter));
        Builder.AddAttribute(9, "onmouseleave", EventCallback.Factory.Create<MouseEventArgs>(this, OnMouseLeave));

        if (m_IsSvg)
        {
            Builder.OpenElement(10, "rect");
            Builder.AddAttribute(11, "width", Group.Size!.Width);
            Builder.AddAttribute(12, "height", Group.Size.Height);
            Builder.AddAttribute(13, "fill", "none");
            Builder.CloseElement();
        }

        Builder.OpenComponent(14, t_ComponentType);
        Builder.AddAttribute(15, "Group", Group);
        Builder.CloseComponent();
        Builder.CloseElement();
    }

    private void OnPointerDown(PointerEventArgs E)
    {
        BlazorDiagram.TriggerPointerDown(Group, E.ToCore());
    }

    private void OnPointerUp(PointerEventArgs E)
    {
        BlazorDiagram.TriggerPointerUp(Group, E.ToCore());
    }

    private void OnMouseEnter(MouseEventArgs E)
    {
        BlazorDiagram.TriggerPointerEnter(Group, E.ToCore());
    }

    private void OnMouseLeave(MouseEventArgs E)
    {
        BlazorDiagram.TriggerPointerLeave(Group, E.ToCore());
    }
}