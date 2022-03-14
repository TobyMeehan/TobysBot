using System.Text;

namespace TobysBot.Discord.Client.Extensions;

public static class StringBuilderExtensions
{
    public static StringBuilder Prepend(this StringBuilder sb, string value)
    {
        return sb.Insert(0, value);
    }

    public static StringBuilder PrependLine(this StringBuilder sb, string value)
    {
        return sb.Prepend($"{value}\n");
    } 
}