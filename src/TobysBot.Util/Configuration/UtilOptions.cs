using Microsoft.Extensions.Options;

namespace TobysBot.Util.Configuration;

public class UtilOptions
{
    public UtilDataOptions Data { get; set; }
}

public class UtilDataOptions
{
    public string? ReminderCollection { get; set; }
}