using System.Text;

namespace Trivial.Graph.Extensions;

public static class StringBuilderExtensions
{
    public static StringBuilder AppendIf(this StringBuilder builder, string str, bool condition)
    {
        if (condition) builder.Append(str);

        return builder;
    }
}