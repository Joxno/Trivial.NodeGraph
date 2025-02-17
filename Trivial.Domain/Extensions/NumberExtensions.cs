using System.Globalization;

namespace Trivial.Domain.Extensions;

public static class NumberExtensions
{
    public static string ToInvariantString(this float N) => N.ToString(CultureInfo.InvariantCulture);
}
