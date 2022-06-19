using System.Text;

namespace TobysBot.Music.Extensions;

public static class StringBuilderExtensions
{
    public static StringBuilder Prepend(this StringBuilder sb, string value)
    {
        return sb.Insert(0, value);
    }

    public static StringBuilder PrependLine(this StringBuilder sb)
    {
        return sb.PrependLine("");
    }

    public static StringBuilder PrependLine(this StringBuilder sb, string value)
    {
        return sb.Prepend($"{value}\n");
    }
}