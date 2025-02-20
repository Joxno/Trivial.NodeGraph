﻿using Trivial.Graph.Domain.Behaviors;
using Trivial.Graph.Domain.Events;
using Trivial.Graph.Domain.Geometry;
using Trivial.Graph.Domain.Models;
using Trivial.Graph.Domain.Models.Base;
using Trivial.Graph.Domain.Positions;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Trivial.Graph.Domain.exceptions;

namespace Trivial.Graph.Domain.Controls.Default;

public class ArrowHeadControl : ExecutableControl
{
    public ArrowHeadControl(bool Source, LinkMarker? Marker = null)
    {
        this.Source = Source;
        this.Marker = Marker ?? LinkMarker.NewArrow(20, 20);
    }

    public bool Source { get; }
    public LinkMarker Marker { get; }
    public float Angle { get; private set; }

    public override Vector2? GetPosition(Model Model)
    {
        if (Model is not BaseLinkModel t_Link)
            throw new DiagramsException("ArrowHeadControl only works for models of type BaseLinkModel");

        var t_Dist = Source ? Marker.Width - (t_Link.SourceMarker?.Width ?? 0) : (t_Link.TargetMarker?.Width ?? 0) - Marker.Width;
        var t_Pp = new LinkPathPositionProvider(t_Dist);
        var t_P1 = t_Pp.GetPosition(t_Link);
        if (t_P1 is not null)
        {
            var t_P2 = Source ? t_Link.Source.GetPosition(t_Link) : t_Link.Target.GetPosition(t_Link);
            if (t_P2 is not null)
            {
                Angle = MathF.Atan2(t_P2.Value.Y - t_P1.Value.Y, t_P2.Value.X - t_P1.Value.X) * 180 / MathF.PI;
            }
        }

        return t_P1;
    }

    public override ValueTask OnPointerDown(Diagram Diagram, Model Model, PointerEventArgs E)
    {
        if (Model is not BaseLinkModel t_Link)
            throw new DiagramsException("ArrowHeadControl only works for models of type BaseLinkModel");

        var t_Dnlb = Diagram.GetBehavior<DragNewLinkBehavior>()!;
        if (Source)
        {
            t_Link.SetSource(t_Link.Target);
        }

        t_Dnlb.StartFrom(t_Link, E.GetClientPos());
        return ValueTask.CompletedTask;
    }
}
