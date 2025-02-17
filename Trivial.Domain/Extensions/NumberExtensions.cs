using System.Globalization;

namespace Trivial.Domain.Extensions;

public static class NumberExtensions
{
    public static string ToInvariantString(this float n) => n.ToString(CultureInfo.InvariantCulture);
}
