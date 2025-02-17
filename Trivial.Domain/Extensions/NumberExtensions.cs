using System.Globalization;

namespace Trivial.Graph.Domain.Extensions;

public static class NumberExtensions
{
    public static string ToInvariantString(this float N) => N.ToString(CultureInfo.InvariantCulture);
}
