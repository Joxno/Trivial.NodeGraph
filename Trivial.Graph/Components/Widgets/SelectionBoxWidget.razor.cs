using System;
using System.Numerics;
using Trivial.Domain.Events;
using Trivial.Domain.Geometry;
using Trivial.Domain.Models.Base;
using Microsoft.AspNetCore.Components;

namespace Trivial.Graph.Components.Widgets;

public partial class SelectionBoxWidget : IDisposable
{
    private Vector2? m_InitialClientPoint;
    private Size? m_SelectionBoxSize; // Todo: remove unneeded instantiations
    private Vector2? m_SelectionBoxTopLeft; // Todo: remove unneeded instantiations

    [CascadingParameter] public BlazorDiagram BlazorDiagram { get; set; } = null!;

    [Parameter] public string Background { get; set; } = "rgb(110 159 212 / 25%)";

    public void Dispose()
    {
        BlazorDiagram.PointerDown -= OnPointerDown;
        BlazorDiagram.PointerMove -= OnPointerMove;
        BlazorDiagram.PointerUp -= OnPointerUp;
    }

    protected override void OnInitialized()
    {
        BlazorDiagram.PointerDown += OnPointerDown;
        BlazorDiagram.PointerMove += OnPointerMove;
        BlazorDiagram.PointerUp += OnPointerUp;
    }

    private string GenerateStyle()
    {
        return FormattableString.Invariant(
            $"position: absolute; background: {Background}; top: {m_SelectionBoxTopLeft!.Value.Y}px; left: {m_SelectionBoxTopLeft.Value.X}px; width: {m_SelectionBoxSize!.Width}px; height: {m_SelectionBoxSize.Height}px;");
    }

    private void OnPointerDown(Model? Model, MouseEventArgs E)
    {
        if (Model != null || !E.ShiftKey)
            return;

        m_InitialClientPoint = new Vector2(E.ClientX, E.ClientY);
    }

    private void OnPointerMove(Model? Model, MouseEventArgs E)
    {
        if (m_InitialClientPoint == null)
            return;

        SetSelectionBoxInformation(E);

        var t_Start = BlazorDiagram.GetRelativeMousePoint(m_InitialClientPoint.Value.X, m_InitialClientPoint.Value.Y);
        var t_End = BlazorDiagram.GetRelativeMousePoint(E.ClientX, E.ClientY);
        var (t_SX, t_SY) = (MathF.Min(t_Start.X, t_End.X), MathF.Min(t_Start.Y, t_End.Y));
        var (t_EX, t_EY) = (MathF.Max(t_Start.X, t_End.X), MathF.Max(t_Start.Y, t_End.Y));
        var t_Bounds = new Rectangle(t_SX, t_SY, t_EX, t_EY);

        foreach (var t_Node in BlazorDiagram.Nodes)
        {
            var t_NodeBounds = t_Node.GetBounds();
            if (t_NodeBounds == null)
                continue;

            if (t_Bounds.Overlap(t_NodeBounds))
                BlazorDiagram.SelectModel(t_Node, false);
            else if (t_Node.Selected) BlazorDiagram.UnselectModel(t_Node);
        }

        InvokeAsync(StateHasChanged);
    }

    private void SetSelectionBoxInformation(MouseEventArgs E)
    {
        var t_Start = BlazorDiagram.GetRelativePoint(m_InitialClientPoint!.Value.X, m_InitialClientPoint.Value.Y);
        var t_End = BlazorDiagram.GetRelativePoint(E.ClientX, E.ClientY);
        var (t_SX, t_SY) = (MathF.Min(t_Start.X, t_End.X), MathF.Min(t_Start.Y, t_End.Y));
        var (t_EX, t_EY) = (MathF.Max(t_Start.X, t_End.X), MathF.Max(t_Start.Y, t_End.Y));
        m_SelectionBoxTopLeft = new Vector2(t_SX, t_SY);
        m_SelectionBoxSize = new Size(t_EX - t_SX, t_EY - t_SY);
    }

    private void OnPointerUp(Model? Model, MouseEventArgs E)
    {
        m_InitialClientPoint = null;
        m_SelectionBoxTopLeft = null;
        m_SelectionBoxSize = null;
        InvokeAsync(StateHasChanged);
    }
}