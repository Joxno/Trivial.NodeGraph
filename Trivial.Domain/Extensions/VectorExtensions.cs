using System.Numerics;

namespace Trivial.Graph.Domain.Extensions;

public static class VectorExtensions
{
    public static Vector2 Lerp(this Vector2 V1, Vector2 V2, float T) =>
        Vector2.Lerp(V1, V2, T);

    public static float Dot(this Vector2 V1, Vector2 V2) =>
        Vector2.Dot(V1, V2);

    public static float DistanceTo(this Vector2 V1, Vector2 V2) =>
        Vector2.Distance(V1, V2);

    public static Vector2 Normalized(this Vector2 V) =>
        Vector2.Normalize(V);
}