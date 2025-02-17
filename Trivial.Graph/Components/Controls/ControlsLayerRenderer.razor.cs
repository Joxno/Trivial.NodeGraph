using System;
using System.Numerics;
using System.Threading.Tasks;
using Trivial.Domain.Controls;
using Trivial.Domain.Extensions;
using Trivial.Domain.Geometry;
using Trivial.Domain.Models.Base;
using Trivial.Graph.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Trivial.Graph.Components.Controls;

public partial class ControlsLayerRenderer : IDisposable
{
    private bool m_ShouldRender;

    [CascadingParameter] public BlazorDiagram BlazorDiagram { get; set; } = null!;

    [Parameter] public bool Svg { get; set; }

    public void Dispose()
    {
        BlazorDiagram.Controls.ChangeCaused -= OnControlsChangeCaused;
    }

    protected override void OnInitialized()
    {
        BlazorDiagram.Controls.ChangeCaused += OnControlsChangeCaused;
    }

    protected override bool ShouldRender()
    {
        if (!m_ShouldRender)
            return false;

        m_ShouldRender = false;
        return true;
    }

    private void OnControlsChangeCaused(Model Cause)
    {
        if (Svg != Cause.IsSvg())
            return;

        m_ShouldRender = true;
        InvokeAsync(StateHasChanged);
    }

    private RenderFragment RenderControl(Model Model, Control Control, Vector2 Position, bool Svg)
    {
        var t_ComponentType = BlazorDiagram.GetComponent(Control.GetType());
        if (t_ComponentType == null)
            throw new BlazorDiagramsException(
                $"A component couldn't be found for the user action {Control.GetType().Name}");

        return Builder =>
        {
            Builder.OpenElement(0, Svg ? "g" : "div");
            Builder.AddAttribute(1, "class",
                $"{(Control is ExecutableControl ? "executable " : "")}diagram-control {Control.GetType().Name}");
            if (Svg)
                Builder.AddAttribute(2, "transform",
                    $"translate({Position.X.ToInvariantString()} {Position.Y.ToInvariantString()})");
            else
                Builder.AddAttribute(2, "style",
                    $"top: {Position.Y.ToInvariantString()}px; left: {Position.X.ToInvariantString()}px");

            if (Control is ExecutableControl t_Ec)
            {
                Builder.AddAttribute(3, "onpointerdown",
                    EventCallback.Factory.Create<PointerEventArgs>(this, E => OnPointerDown(E, Model, t_Ec)));
                Builder.AddEventStopPropagationAttribute(4, "onpointerdown", true);
            }

            Builder.OpenComponent(5, t_ComponentType);
            Builder.AddAttribute(6, "Control", Control);
            Builder.AddAttribute(7, "Model", Model);
            Builder.CloseComponent();
            Builder.CloseElement();
        };
    }

    private async Task OnPointerDown(PointerEventArgs E, Model Model, ExecutableControl Control)
    {
        if (E.Button == 0 || E.Buttons == 1) await Control.OnPointerDown(BlazorDiagram, Model, E.ToCore());
    }
}