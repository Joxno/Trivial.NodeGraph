using System.Text;
using Trivial.Graph.Domain.Extensions;
using Trivial.Graph.Domain.Geometry;
using Trivial.Graph.Domain.Models;
using Trivial.Graph.Domain.Models.Base;
using Trivial.Graph.Extensions;
using Trivial.Graph.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Trivial.Graph.Components.Renderers;

public class NodeRenderer : ComponentBase, IDisposable
{
    private bool m_BecameVisible;
    private ElementReference m_Element;
    private bool m_IsSvg;
    private DotNetObjectReference<NodeRenderer>? m_Reference;
    private bool m_ShouldRender;

    [CascadingParameter] public BlazorDiagram BlazorDiagram { get; set; } = null!;

    [Parameter] public NodeModel Node { get; set; } = null!;

    [Inject] private IJSRuntime JsRuntime { get; set; } = null!;

    public void Dispose()
    {
        Node.Changed -= OnNodeChanged;
        Node.VisibilityChanged -= OnVisibilityChanged;

        if (m_Element.Id != null && !Node.ControlledSize)
        {
            _ = JsRuntime.UnobserveResizes(m_Element);
        }

        m_Reference?.Dispose();
    }

    [JSInvokable]
    public void OnResize(Size Size)
    {
        if (!Size.IsVisibleSize()) return;

        Size = new Size(Size.Width / BlazorDiagram.Zoom, Size.Height / BlazorDiagram.Zoom);
        if (Node.Size.Width.AlmostEqualTo(Size.Width) &&
            Node.Size.Height.AlmostEqualTo(Size.Height))
        {
            return;
        }

        Node.Size = Size;
        Node.Refresh();
        Node.RefreshLinks();
        Node.ReinitializePorts();
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        m_Reference = DotNetObjectReference.Create(this);
        Node.Changed += OnNodeChanged;
        Node.VisibilityChanged += OnVisibilityChanged;
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        m_IsSvg = Node is SvgNodeModel;
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
        if (!Node.Visible)
            return;

        var t_ComponentType = BlazorDiagram.GetComponent(Node) ??
                            (m_IsSvg ? typeof(SvgNodeWidget) : typeof(NodeWidget));
        var t_Classes = new StringBuilder("diagram-node")
            .AppendIf(" locked", Node.Locked)
            .AppendIf(" selected", Node.Selected)
            .AppendIf(" grouped", Node.Group != null);

        Builder.OpenElement(0, m_IsSvg ? "g" : "div");
        Builder.AddAttribute(1, "class", t_Classes.ToString());
        Builder.AddAttribute(2, "data-node-id", Node.Id);

        if (m_IsSvg)
        {
            Builder.AddAttribute(3, "transform",
                $"translate({Node.Position.X.ToInvariantString()} {Node.Position.Y.ToInvariantString()})");
        }
        else
        {
            Builder.AddAttribute(3, "style",
                $"top: {Node.Position.Y.ToInvariantString()}px; left: {Node.Position.X.ToInvariantString()}px");
        }

        Builder.AddAttribute(4, "onpointerdown", EventCallback.Factory.Create<PointerEventArgs>(this, OnPointerDown));
        Builder.AddEventStopPropagationAttribute(5, "onpointerdown", true);
        Builder.AddAttribute(6, "onpointerup", EventCallback.Factory.Create<PointerEventArgs>(this, OnPointerUp));
        Builder.AddEventStopPropagationAttribute(7, "onpointerup", true);
        Builder.AddAttribute(8, "onmouseenter", EventCallback.Factory.Create<MouseEventArgs>(this, OnMouseEnter));
        Builder.AddAttribute(9, "onmouseleave", EventCallback.Factory.Create<MouseEventArgs>(this, OnMouseLeave));
        Builder.AddElementReferenceCapture(10, Value => m_Element = Value);
        Builder.OpenComponent(11, t_ComponentType);
        Builder.AddAttribute(12, "Node", Node);
        Builder.CloseComponent();

        Builder.CloseElement();
    }

    protected override async Task OnAfterRenderAsync(bool FirstRender)
    {
        if (FirstRender && !Node.Visible)
            return;

        if (FirstRender || m_BecameVisible)
        {
            m_BecameVisible = false;

            if (!Node.ControlledSize)
            {
                await JsRuntime.ObserveResizes(m_Element, m_Reference!);
            }
        }
    }

    private void OnNodeChanged(Model _)
    {
        ReRender();
    }

    private void OnVisibilityChanged(Model _)
    {
        m_BecameVisible = Node.Visible;
        ReRender();
    }

    private void ReRender()
    {
        m_ShouldRender = true;
        InvokeAsync(StateHasChanged);
    }

    private void OnPointerDown(PointerEventArgs E)
    {
        BlazorDiagram.TriggerPointerDown(Node, E.ToCore());
    }

    private void OnPointerUp(PointerEventArgs E)
    {
        BlazorDiagram.TriggerPointerUp(Node, E.ToCore());
    }

    private void OnMouseEnter(MouseEventArgs E)
    {
        BlazorDiagram.TriggerPointerEnter(Node, E.ToCore());
    }

    private void OnMouseLeave(MouseEventArgs E)
    {
        BlazorDiagram.TriggerPointerLeave(Node, E.ToCore());
    }
}