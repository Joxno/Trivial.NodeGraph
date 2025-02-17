using System;
using System.Threading.Tasks;
using Trivial.Graph.Domain.Geometry;
using Trivial.Graph.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Trivial.Graph.Components;

public partial class DiagramCanvas : IAsyncDisposable
{
    private DotNetObjectReference<DiagramCanvas>? m_Reference;
    private bool m_ShouldRender;

    protected ElementReference ElementReference;

    [CascadingParameter] public BlazorDiagram BlazorDiagram { get; set; } = null!;

    [Parameter] public RenderFragment? Widgets { get; set; }

    [Parameter] public RenderFragment? AdditionalSvg { get; set; }

    [Parameter] public RenderFragment? AdditionalHtml { get; set; }

    [Parameter] public string? Class { get; set; }

    [Inject] public IJSRuntime JsRuntime { get; set; } = null!;

    public async ValueTask DisposeAsync()
    {
        BlazorDiagram.Changed -= OnDiagramChanged;

        if (m_Reference == null)
            return;

        if (ElementReference.Id != null)
            await JsRuntime.UnobserveResizes(ElementReference);

        m_Reference.Dispose();
    }

    private string GetLayerStyle(int Order)
    {
        return FormattableString.Invariant(
            $"transform: translate({BlazorDiagram.Pan.X}px, {BlazorDiagram.Pan.Y}px) scale({BlazorDiagram.Zoom}); z-index: {Order};");
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        m_Reference = DotNetObjectReference.Create(this);
        BlazorDiagram.Changed += OnDiagramChanged;
    }

    protected override async Task OnAfterRenderAsync(bool FirstRender)
    {
        await base.OnAfterRenderAsync(FirstRender);

        if (FirstRender)
        {
            BlazorDiagram.SetContainer(await JsRuntime.GetBoundingClientRect(ElementReference));
            await JsRuntime.ObserveResizes(ElementReference, m_Reference!);
        }
    }

    [JSInvokable]
    public void OnResize(Rectangle Rect)
    {
        BlazorDiagram.SetContainer(Rect);
    }

    protected override bool ShouldRender()
    {
        if (!m_ShouldRender) return false;

        m_ShouldRender = false;
        return true;
    }

    private void OnPointerDown(PointerEventArgs E)
    {
        BlazorDiagram.TriggerPointerDown(null, E.ToCore());
    }

    private void OnPointerMove(PointerEventArgs E)
    {
        BlazorDiagram.TriggerPointerMove(null, E.ToCore());
    }

    private void OnPointerUp(PointerEventArgs E)
    {
        BlazorDiagram.TriggerPointerUp(null, E.ToCore());
    }

    private void OnKeyDown(KeyboardEventArgs E)
    {
        BlazorDiagram.TriggerKeyDown(E.ToCore());
    }

    private void OnWheel(WheelEventArgs E)
    {
        BlazorDiagram.TriggerWheel(E.ToCore());
    }

    private void OnDiagramChanged()
    {
        m_ShouldRender = true;
        InvokeAsync(StateHasChanged);
    }
}