using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Trivial.Domain.Geometry;
using Trivial.Domain.Models;
using Trivial.Domain.Models.Base;
using Trivial.Graph.Extensions;
using Trivial.Graph.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Trivial.Graph.Components.Renderers;

public class PortRenderer : ComponentBase, IDisposable
{
    private ElementReference m_Element;
    private bool m_IsParentSvg;
    private bool m_ShouldRefreshPort;
    private bool m_ShouldRender = true;
    private bool m_UpdatingDimensions;

    [CascadingParameter] public BlazorDiagram BlazorDiagram { get; set; } = null!;
    [Inject] private IJSRuntime JsRuntime { get; set; } = null!;
    [Parameter] public PortModel Port { get; set; } = null!;
    [Parameter] public string? Class { get; set; }
    [Parameter] public string? Style { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }

    public void Dispose()
    {
        Port.Changed -= OnPortChanged;
        Port.VisibilityChanged -= OnPortChanged;
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Port.Changed += OnPortChanged;
        Port.VisibilityChanged += OnPortChanged;
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        m_IsParentSvg = Port.Parent is SvgNodeModel;
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
        if (!Port.Visible)
            return;
        
        Builder.OpenElement(0, m_IsParentSvg ? "g" : "div");
        Builder.AddAttribute(1, "style", Style);
        Builder.AddAttribute(2, "class",
            "diagram-port" + " " + Port.Alignment.ToString().ToLower() + " " + (Port.Links.Count > 0 ? "has-links" : "") + " " +
            Class);
        Builder.AddAttribute(3, "data-port-id", Port.Id);
        Builder.AddAttribute(4, "onpointerdown", EventCallback.Factory.Create<PointerEventArgs>(this, OnPointerDown));
        Builder.AddEventStopPropagationAttribute(5, "onpointerdown", true);
        Builder.AddAttribute(6, "onpointerup", EventCallback.Factory.Create<PointerEventArgs>(this, OnPointerUp));
        Builder.AddEventStopPropagationAttribute(7, "onpointerup", true);
        Builder.AddElementReferenceCapture(8, Value => { m_Element = Value; });
        Builder.AddContent(9, ChildContent);
        Builder.CloseElement();
    }

    protected override async Task OnAfterRenderAsync(bool FirstRender)
    {
        if (!Port.Initialized)
        {
            await UpdateDimensions();
        }
    }

    private void OnPointerDown(PointerEventArgs E)
    {
        BlazorDiagram.TriggerPointerDown(Port, E.ToCore());
    }

    private void OnPointerUp(PointerEventArgs E)
    {
        var t_Model = E.PointerType == "mouse" ? Port : FindPortOn((float)E.ClientX, (float)E.ClientY);
        BlazorDiagram.TriggerPointerUp(t_Model, E.ToCore());
    }

    private PortModel? FindPortOn(float ClientX, float ClientY)
    {
        var t_AllPorts = BlazorDiagram.Nodes.SelectMany(N => N.Ports)
            .Union(BlazorDiagram.Groups.SelectMany(G => G.Ports));

        foreach (var t_Port in t_AllPorts)
        {
            if (!t_Port.Initialized)
                continue;

            var t_RelativePt = BlazorDiagram.GetRelativeMousePoint(ClientX, ClientY);
            if (t_Port.GetBounds().ContainsPoint(t_RelativePt))
                return t_Port;
        }

        return null;
    }

    private async Task UpdateDimensions()
    {
        if (BlazorDiagram.Container == null)
            return;

        m_UpdatingDimensions = true;
        var t_Zoom = BlazorDiagram.Zoom;
        var t_Pan = BlazorDiagram.Pan;
        var t_Rect = await JsRuntime.GetBoundingClientRect(m_Element);

        Port.Size = new Size(t_Rect.Width / t_Zoom, t_Rect.Height / t_Zoom);
        Port.Position = new Vector2((t_Rect.Left - BlazorDiagram.Container.Left - t_Pan.X) / t_Zoom,
            (t_Rect.Top - BlazorDiagram.Container.Top - t_Pan.Y) / t_Zoom);

        Port.Initialized = true;
        m_UpdatingDimensions = false;

        if (m_ShouldRefreshPort)
        {
            m_ShouldRefreshPort = false;
            Port.RefreshAll();
        }
        else
        {
            Port.RefreshLinks();
        }
    }

    private async void OnPortChanged(Model _)
    {
        // If an update is ongoing and the port is refreshed again,
        // it's highly likely the port needs to be refreshed (e.g. link added)
        if (m_UpdatingDimensions) m_ShouldRefreshPort = true;

        if (Port.Initialized)
        {
            m_ShouldRender = true;
            await InvokeAsync(StateHasChanged);
        }
        else
        {
            await UpdateDimensions();
        }
    }
}