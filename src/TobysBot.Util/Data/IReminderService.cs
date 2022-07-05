using Discord;

namespace TobysBot.Util.Data;

public interface IReminderService
{
    Task<IReadOnlyCollection<IReminder>> GetRemindersAsync();

    Task<IReadOnlyCollection<IReminder>> ListRemindersAsync(IUser user);

    Task DeleteReminderAsync(string id);
}