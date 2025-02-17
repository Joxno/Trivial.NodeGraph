using System.Numerics;
using Trivial.Domain.Geometry;
using SvgPathProperties;

namespace Trivial.Domain;

public class PathGeneratorResult
{
    public PathGeneratorResult(SvgPath FullPath, SvgPath[] Paths, float? SourceMarkerAngle = null, Vector2? SourceMarkerPosition = null,
        float? TargetMarkerAngle = null, Vector2? TargetMarkerPosition = null)
    {
        this.FullPath = FullPath;
        this.Paths = Paths;
        this.SourceMarkerAngle = SourceMarkerAngle;
        this.SourceMarkerPosition = SourceMarkerPosition;
        this.TargetMarkerAngle = TargetMarkerAngle;
        this.TargetMarkerPosition = TargetMarkerPosition;
    }

    public SvgPath FullPath { get; }
    public SvgPath[] Paths { get; }
    public float? SourceMarkerAngle { get; }
    public Vector2? SourceMarkerPosition { get; }
    public float? TargetMarkerAngle { get; }
    public Vector2? TargetMarkerPosition { get; }
}
