using Discord;

namespace TobysBot.Util.Data;

public interface IReminderService
{
    Task CreateReminderAsync(IUser user, DateTimeOffset triggerAt, string name);
    Task<IReadOnlyCollection<IReminder>> ListRemindersAsync();

    Task<IReadOnlyCollection<IReminder>> ListRemindersAsync(IUser user);

    Task DeleteReminderAsync(string id);
}