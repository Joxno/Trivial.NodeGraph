using Trivial.Domain.Anchors;
using Trivial.Domain.Geometry;
using System;
using System.Collections.Generic;
using System.Numerics;
using Trivial.Domain.PathGenerators;
using Trivial.Domain.Routers;

namespace Trivial.Domain.Models.Base;

public abstract class BaseLinkModel : SelectableModel, IHasBounds, ILinkable
{
    private readonly List<BaseLinkModel> m_Links = new();

    public event Action<BaseLinkModel, Anchor, Anchor>? SourceChanged;
    public event Action<BaseLinkModel, Anchor, Anchor>? TargetChanged;
    public event Action<BaseLinkModel>? TargetAttached;

    protected BaseLinkModel(Anchor Source, Anchor Target)
    {
        this.Source = Source;
        this.Target = Target;
    }

    protected BaseLinkModel(string Id, Anchor Source, Anchor Target) : base(Id)
    {
        this.Source = Source;
        this.Target = Target;
    }

    public Anchor Source { get; private set; }
    public Anchor Target { get; private set; }
    public Diagram? Diagram { get; internal set; }
    public Vector2[]? Route { get; private set; }
    public PathGeneratorResult? PathGeneratorResult { get; private set; }
    public bool IsAttached => Source is not PositionAnchor && Target is not PositionAnchor;
    public Router? Router { get; set; }
    public PathGenerator? PathGenerator { get; set; }
    public LinkMarker? SourceMarker { get; set; }
    public LinkMarker? TargetMarker { get; set; }
    public bool Segmentable { get; set; } = false;
    public List<LinkVertexModel> Vertices { get; } = new();
    public List<LinkLabelModel> Labels { get; } = new();
    public IReadOnlyList<BaseLinkModel> Links => m_Links;

    public override void Refresh()
    {
        GeneratePath();
        base.Refresh();
    }

    public void RefreshLinks()
    {
        foreach (var t_Link in Links)
        {
            t_Link.Refresh();
        }
    }

    public LinkLabelModel AddLabel(string Content, float? Distance = null, Vector2? Offset = null)
    {
        var t_Label = new LinkLabelModel(this, Content, Distance, Offset);
        Labels.Add(t_Label);
        return t_Label;
    }

    public LinkVertexModel AddVertex(Vector2? Position = null)
    {
        var t_Vertex = new LinkVertexModel(this, Position);
        Vertices.Add(t_Vertex);
        return t_Vertex;
    }

    public void SetSource(Anchor Anchor)
    {
        ArgumentNullException.ThrowIfNull(Anchor, nameof(Anchor));

        if (Source == Anchor)
            return;

        var t_Old = Source;
        Source = Anchor;
        SourceChanged?.Invoke(this, t_Old, Source);
    }

    public void SetTarget(Anchor Anchor)
    {
        if (Target == Anchor)
            return;

        var t_Old = Target;
        Target = Anchor;
        TargetChanged?.Invoke(this, t_Old, Target);
    }

    public Rectangle? GetBounds()
    {
        if (PathGeneratorResult == null)
            return null;

        var t_MinX = float.PositiveInfinity;
        var t_MinY = float.PositiveInfinity;
        var t_MaxX = float.NegativeInfinity;
        var t_MaxY = float.NegativeInfinity;

        var t_Path = PathGeneratorResult.FullPath;
        var t_Bbox = t_Path.GetBBox();
        t_MinX = MathF.Min(t_MinX, (float)t_Bbox.Left);
        t_MinY = MathF.Min(t_MinY, (float)t_Bbox.Top);
        t_MaxX = MathF.Max(t_MaxX, (float)t_Bbox.Right);
        t_MaxY = MathF.Max(t_MaxY, (float)t_Bbox.Bottom);

        return new Rectangle(t_MinX, t_MinY, t_MaxX, t_MaxY);
    }

    public bool CanAttachTo(ILinkable Other) => true;

    /// <summary>
    /// Triggers the TargetAttached event
    /// </summary>
    public void TriggerTargetAttached() => TargetAttached?.Invoke(this);

    private void GeneratePath()
    {
        if (Diagram != null)
        {
            var t_Router = Router ?? Diagram.Options.Links.DefaultRouter;
            var t_PathGenerator = PathGenerator ?? Diagram.Options.Links.DefaultPathGenerator;
            var t_Route = t_Router.GetRoute(Diagram, this);
            var t_Source = Source.GetPosition(this, t_Route);
            var t_Target = Target.GetPosition(this, t_Route);
            if (t_Source != null && t_Target != null)
            {
                Route = t_Route;
                PathGeneratorResult = t_PathGenerator.GetResult(Diagram, this, t_Route, t_Source.Value, t_Target.Value);
                return;
            }
        }

        Route = null;
        PathGeneratorResult = null;
    }

    void ILinkable.AddLink(BaseLinkModel Link) => m_Links.Add(Link);

    void ILinkable.RemoveLink(BaseLinkModel Link) => m_Links.Remove(Link);
}
