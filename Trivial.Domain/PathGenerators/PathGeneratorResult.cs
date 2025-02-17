using System.Numerics;
using Trivial.Domain.Geometry;
using SvgPathProperties;

namespace Trivial.Domain;

public class PathGeneratorResult
{
    public PathGeneratorResult(SvgPath fullPath, SvgPath[] paths, float? sourceMarkerAngle = null, Vector2? sourceMarkerPosition = null,
        float? targetMarkerAngle = null, Vector2? targetMarkerPosition = null)
    {
        FullPath = fullPath;
        Paths = paths;
        SourceMarkerAngle = sourceMarkerAngle;
        SourceMarkerPosition = sourceMarkerPosition;
        TargetMarkerAngle = targetMarkerAngle;
        TargetMarkerPosition = targetMarkerPosition;
    }

    public SvgPath FullPath { get; }
    public SvgPath[] Paths { get; }
    public float? SourceMarkerAngle { get; }
    public Vector2? SourceMarkerPosition { get; }
    public float? TargetMarkerAngle { get; }
    public Vector2? TargetMarkerPosition { get; }
}
