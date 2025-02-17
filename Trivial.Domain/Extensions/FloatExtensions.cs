using System;

namespace Trivial.Domain.Extensions;

public static class FloatExtensions
{
    public static bool AlmostEqualTo(this float float1, float float2, float tolerance = 0.0001f)
        => MathF.Abs(float1 - float2) < tolerance;
}
