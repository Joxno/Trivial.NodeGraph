using System;

namespace Trivial.Graph.Domain.Extensions;

public static class FloatExtensions
{
    public static bool AlmostEqualTo(this float Float1, float Float2, float Tolerance = 0.0001f)
        => MathF.Abs(Float1 - Float2) < Tolerance;
}
