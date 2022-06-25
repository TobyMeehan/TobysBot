using System.Text.RegularExpressions;

namespace TobysBot.Extensions;

public static class StringExtensions
{
    public static bool HasSpecialCharacters(this string value)
    {
        return !new Regex("^[a-zA-Z0-9 ]*$").IsMatch(value);
    }
}