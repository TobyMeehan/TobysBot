using TobysBot.Data;

namespace TobysBot.Util;

public interface IReminder : INamedEntity, IUserRelation
{
    DateTimeOffset TriggerAt { get; }
}