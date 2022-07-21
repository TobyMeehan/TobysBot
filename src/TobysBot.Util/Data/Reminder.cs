using TobysBot.Data;

namespace TobysBot.Util.Data;

public class Reminder : Entity, IReminder
{
    public Reminder(string name, DateTimeOffset triggerAt)
    {
        Name = name;
        TriggerAt = triggerAt;
    }
    
    public string? Name { get; set; }
    public ulong UserId { get; set; }
    public DateTimeOffset TriggerAt { get; set; }
}