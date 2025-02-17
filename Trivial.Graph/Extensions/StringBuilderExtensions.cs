using System.Text;

namespace Trivial.Graph.Extensions;

public static class StringBuilderExtensions
{
    public static StringBuilder AppendIf(this StringBuilder Builder, string Str, bool Condition)
    {
        if (Condition) Builder.Append(Str);

        return Builder;
    }
}