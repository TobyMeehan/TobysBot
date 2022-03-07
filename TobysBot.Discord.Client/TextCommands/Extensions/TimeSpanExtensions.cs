using System;
using System.Text;

namespace TobysBot.Discord.Client.TextCommands.Extensions;

public static class TimeSpanExtensions
{
    public static string ToTimeString(this TimeSpan timeSpan)
    {
        StringBuilder sb = new StringBuilder();

        if (timeSpan.TotalDays >= 1)
        {
            sb.Append($"{timeSpan.Days:D2}:");
        }

        if (timeSpan.TotalHours >= 1)
        {
            sb.Append($"{timeSpan.Hours:D2}:");
        }

        sb.Append($"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}");

        return sb.ToString();
    }
}