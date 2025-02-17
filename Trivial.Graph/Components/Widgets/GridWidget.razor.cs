using System;
using System.Text;
using Trivial.Graph.Domain.Extensions;
using Microsoft.AspNetCore.Components;

namespace Trivial.Graph.Components.Widgets;

public partial class GridWidget : IDisposable
{
    private bool m_Visible;
    private float m_ScaledSize;
    private float m_PosX;
    private float m_PosY;
    
    [CascadingParameter] public BlazorDiagram BlazorDiagram { get; set; } = null!;
    [Parameter] public float Size { get; set; } = 20;
    [Parameter] public float ZoomThreshold { get; set; } = 0;
    [Parameter] public GridMode Mode { get; set; } = GridMode.Line;
    [Parameter] public string BackgroundColor { get; set; } = "rgb(241 241 241)";

    public void Dispose()
    {
        BlazorDiagram.PanChanged -= RefreshPosition;
        BlazorDiagram.ZoomChanged -= RefreshPosition;
    }

    protected override void OnInitialized()
    {
        BlazorDiagram.PanChanged += RefreshPosition;
        BlazorDiagram.ZoomChanged += RefreshPosition;
    }

    protected override void OnParametersSet()
    {
        m_PosX = BlazorDiagram.Pan.X;
        m_PosY = BlazorDiagram.Pan.Y;
        m_ScaledSize = Size * BlazorDiagram.Zoom;
        m_Visible = BlazorDiagram.Zoom > ZoomThreshold;
    }

    private void RefreshPosition()
    {
        m_PosX = BlazorDiagram.Pan.X;
        m_PosY = BlazorDiagram.Pan.Y;
        m_ScaledSize = Size * BlazorDiagram.Zoom;
        m_Visible = BlazorDiagram.Zoom > ZoomThreshold;
        InvokeAsync(StateHasChanged);
    }

    private string GenerateStyle()
    {
        var t_Sb = new StringBuilder();

        t_Sb.Append($"background-color: {BackgroundColor};");
        t_Sb.Append($"background-size: {m_ScaledSize.ToInvariantString()}px {m_ScaledSize.ToInvariantString()}px;");
        t_Sb.Append($"background-position-x: {m_PosX.ToInvariantString()}px;");
        t_Sb.Append($"background-position-y: {m_PosY.ToInvariantString()}px;");

        switch (Mode)
        {
            case GridMode.Line:
                t_Sb.Append("background-image: linear-gradient(rgb(211, 211, 211) 1px, transparent 1px), linear-gradient(90deg, rgb(211, 211, 211) 1px, transparent 1px);");
                break;
            case GridMode.Point:
                t_Sb.Append("background-image: radial-gradient(circle at 0 0, rgb(129, 129, 129) 1px, transparent 1px);");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        return t_Sb.ToString();
    }
}

public enum GridMode
{
    Line,
    Point
}