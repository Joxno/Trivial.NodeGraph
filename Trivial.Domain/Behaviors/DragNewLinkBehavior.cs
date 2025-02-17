using Trivial.Domain.Models;
using Trivial.Domain.Models.Base;
using Trivial.Domain.Events;
using System.Linq;
using System.Numerics;
using Trivial.Domain.Anchors;
using Trivial.Domain.Geometry;
using Trivial.Domain.Extensions;

namespace Trivial.Domain.Behaviors;

public class DragNewLinkBehavior : Behavior
{
    private PositionAnchor? m_TargetPositionAnchor;

    public BaseLinkModel? OngoingLink { get; private set; }

    public DragNewLinkBehavior(Diagram Diagram) : base(Diagram)
    {
        base.Diagram.PointerDown += OnPointerDown;
        base.Diagram.PointerMove += OnPointerMove;
        base.Diagram.PointerUp += OnPointerUp;
    }

    public void StartFrom(ILinkable Source, float ClientX, float ClientY)
    {
        if (OngoingLink != null)
            return;

        m_TargetPositionAnchor = new PositionAnchor(CalculateTargetPosition(ClientX, ClientY));
        OngoingLink = Diagram.Options.Links.Factory(Diagram, Source, m_TargetPositionAnchor);
        if (OngoingLink == null)
            return;

        Diagram.Links.Add(OngoingLink);
    }

    public void StartFrom(BaseLinkModel Link, float ClientX, float ClientY)
    {
        if (OngoingLink != null)
            return;

        m_TargetPositionAnchor = new PositionAnchor(CalculateTargetPosition(ClientX, ClientY));
        OngoingLink = Link;
        OngoingLink.SetTarget(m_TargetPositionAnchor);
        OngoingLink.Refresh();
        OngoingLink.RefreshLinks();
    }

    private void OnPointerDown(Model? Model, MouseEventArgs E)
    {
        if (E.Button != (int)MouseEventButton.Left)
            return;

        OngoingLink = null;
        m_TargetPositionAnchor = null;

        if (Model is PortModel t_Port)
        {
            if (t_Port.Locked)
                return;

            m_TargetPositionAnchor = new PositionAnchor(CalculateTargetPosition(E.ClientX, E.ClientY));
            OngoingLink = Diagram.Options.Links.Factory(Diagram, t_Port, m_TargetPositionAnchor);
            if (OngoingLink == null)
                return;

            OngoingLink.SetTarget(m_TargetPositionAnchor);
            Diagram.Links.Add(OngoingLink);
        }
    }

    private void OnPointerMove(Model? Model, MouseEventArgs E)
    {
        if (OngoingLink == null || Model != null)
            return;

        m_TargetPositionAnchor!.SetPosition(CalculateTargetPosition(E.ClientX, E.ClientY));

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

    private void OnPointerUp(Model? Model, MouseEventArgs E)
    {
        if (OngoingLink == null)
            return;

        if (OngoingLink.IsAttached) // Snapped already
        {
            OngoingLink.TriggerTargetAttached();
            OngoingLink = null;
            return;
        }

        if (Model is ILinkable t_Linkable && (OngoingLink.Source.Model == null || OngoingLink.Source.Model.CanAttachTo(t_Linkable)))
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

    private Vector2 CalculateTargetPosition(float ClientX, float ClientY)
    {
        var t_Target = Diagram.GetRelativeMousePoint(ClientX, ClientY);

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

    public override void Dispose()
    {
        Diagram.PointerDown -= OnPointerDown;
        Diagram.PointerMove -= OnPointerMove;
        Diagram.PointerUp -= OnPointerUp;
    }
}