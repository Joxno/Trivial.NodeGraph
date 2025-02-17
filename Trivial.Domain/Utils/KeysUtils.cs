using System.Text;

namespace Trivial.Domain.Utils;

public static class KeysUtils
{
    public static string GetStringRepresentation(bool Ctrl, bool Shift, bool Alt, string Key)
    {
        var t_Sb = new StringBuilder();

        if (Ctrl) t_Sb.Append("Ctrl+");
        if (Shift) t_Sb.Append("Shift+");
        if (Alt) t_Sb.Append("Alt+");
        t_Sb.Append(Key);

        return t_Sb.ToString();
    }
}
