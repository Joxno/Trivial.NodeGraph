using Trivial.Graph.Domain.Models;
using Trivial.Graph.Domain.Models.Base;
using Trivial.Graph.Domain.Events;
using System.Numerics;
using Trivial.Graph.Domain.Anchors;
using Trivial.Graph.Domain.Extensions;

namespace Trivial.Graph.Domain.Behaviors;

public class DragNewLinkBehavior : BaseBehaviour
{
    private PositionAnchor? m_TargetPositionAnchor;

    public BaseLinkModel? OngoingLink { get; private set; }
    public MouseEventButton DragButton { get; set; } = MouseEventButton.Left;
    public Maybe<LinkFactory> LinkFactory { get; set; } = Maybe.None;

    public DragNewLinkBehavior(Diagram Diagram) : base(Diagram) {}

    public void StartFrom(ILinkable Source, Vector2 Pos)
    {
        if (OngoingLink != null) return;

        m_TargetPositionAnchor = new PositionAnchor(CalculateTargetPosition(Pos));
        OngoingLink = LinkFactory.Map(F => F(Diagram, Source, m_TargetPositionAnchor))
            .ValueOrLazy(() => Diagram.Options.Links.Factory(Diagram, Source, m_TargetPositionAnchor));

        if (OngoingLink == null) return;
        Diagram.Links.Add(OngoingLink);
    }

    public void StartFrom(BaseLinkModel Link, Vector2 Pos)
    {
        if (OngoingLink != null) return;

        m_TargetPositionAnchor = new PositionAnchor(CalculateTargetPosition(Pos));
        OngoingLink = Link;
        OngoingLink.SetTarget(m_TargetPositionAnchor);
        OngoingLink.Refresh();
        OngoingLink.RefreshLinks();
    }

    protected override void _OnPointerDown(Maybe<Model> Model, PointerEventArgs E)
    {
        if (E.Button != (int)DragButton) return;

        OngoingLink = null;
        m_TargetPositionAnchor = null;

        Model.Then(M => {
            if (M is PortModel t_Port)
            {
                if (t_Port.Locked)
                    return;

                m_TargetPositionAnchor = new PositionAnchor(CalculateTargetPosition(E.GetClientPos()));
                OngoingLink = LinkFactory.Map(F => F(Diagram, t_Port, m_TargetPositionAnchor))
                    .ValueOrLazy(() => Diagram.Options.Links.Factory(Diagram, t_Port, m_TargetPositionAnchor));
                if (OngoingLink == null)
                    return;

                OngoingLink.SetTarget(m_TargetPositionAnchor);
                Diagram.Links.Add(OngoingLink);
            }
        });
    }

    protected override void _OnPointerMove(Maybe<Model> Model, PointerEventArgs E)
    {
        if (OngoingLink == null || Model.HasValue) return;

        m_TargetPositionAnchor!.SetPosition(CalculateTargetPosition(E.GetClientPos()));

        if (Diagram.Options.Links.EnableSnapping)
        {
            var t_NearPort = FindNearPortToAttachTo();
            if (t_NearPort != null || OngoingLink.Target is not PositionAnchor)
            {
                OngoingLink.SetTarget(t_NearPort is null ? m_TargetPositionAnchor : new SinglePortAnchor(t_NearPort));
            }
        }

        OngoingLink.Refresh();
        OngoingLink.RefreshLinks();
    }

    protected override void _OnPointerUp(Maybe<Model> Model, PointerEventArgs E)
    {
        if (OngoingLink == null)
            return;

        if (OngoingLink.IsAttached) // Snapped already
        {
            OngoingLink.TriggerTargetAttached();
            OngoingLink = null;
            return;
        }

        if (Model.HasValue && Model.Value is ILinkable t_Linkable && (OngoingLink.Source.Model == null || OngoingLink.Source.Model.CanAttachTo(t_Linkable)))
        {
            var t_TargetAnchor = Diagram.Options.Links.TargetAnchorFactory(Diagram, OngoingLink, t_Linkable);
            OngoingLink.SetTarget(t_TargetAnchor);
            OngoingLink.TriggerTargetAttached();
            OngoingLink.Refresh();
            OngoingLink.RefreshLinks();
        }
        else if (Diagram.Options.Links.RequireTarget)
        {
            Diagram.Links.Remove(OngoingLink);
        }
        else if (!Diagram.Options.Links.RequireTarget)
        {
            OngoingLink.Refresh();
        }

        OngoingLink = null;
    }

    private Vector2 CalculateTargetPosition(Vector2 Pos)
    {
        var t_Target = Diagram.GetRelativeMousePoint(Pos.X, Pos.Y);

        if (OngoingLink == null)
        {
            return t_Target;
        }

        var t_Source = OngoingLink.Source.GetPlainPosition()!;
        var t_DirVector = (t_Target - t_Source.Value).Normalized();
        var t_Change = t_DirVector * 5;
        return t_Target - t_Change;
    }

    private PortModel? FindNearPortToAttachTo()
    {
        if (OngoingLink is null || m_TargetPositionAnchor is null)
            return null;

        PortModel? t_NearestSnapPort = null;
        var t_NearestSnapPortDistance = float.PositiveInfinity;

        var t_Position = m_TargetPositionAnchor!.GetPosition(OngoingLink)!;

        foreach (var t_Port in Diagram.Nodes.SelectMany((NodeModel N) => N.Ports))
        {
            var t_Distance = t_Position.Value.DistanceTo(t_Port.Position);

            if (t_Distance <= Diagram.Options.Links.SnappingRadius && (OngoingLink.Source.Model?.CanAttachTo(t_Port) != false))
            {
                if (t_Distance < t_NearestSnapPortDistance)
                {
                    t_NearestSnapPortDistance = t_Distance;
                    t_NearestSnapPort = t_Port;
                }
            }
        }

        return t_NearestSnapPort;
    }
}