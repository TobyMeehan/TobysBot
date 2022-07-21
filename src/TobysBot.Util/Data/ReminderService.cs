using Discord;
using Microsoft.Extensions.Options;
using TobysBot.Data;
using TobysBot.Util.Configuration;

namespace TobysBot.Util.Data;

public class ReminderService : IReminderService
{
    private readonly IDataAccess _dataAccess;
    private readonly UtilDataOptions _options;

    public ReminderService(IDataAccess dataAccess, IOptions<UtilOptions> options)
    {
        _dataAccess = dataAccess;
        _options = options.Value.Data ?? throw new NullReferenceException("No util data options specified.");;
    }

    public async Task CreateReminderAsync(IUser user, DateTimeOffset triggerAt, string name)
    {
        if (_options.ReminderCollection is null)
        {
            throw new NullReferenceException("Saved queue collection name not specified.");
        }

        await _dataAccess.SaveByUserAsync(_options.ReminderCollection, new Reminder(name, triggerAt));
    }

    public async Task<IReadOnlyCollection<IReminder>> ListRemindersAsync()
    {
        if (_options.ReminderCollection is null)
        {
            throw new NullReferenceException("Saved queue collection name not specified.");
        }
        
        return await _dataAccess.GetAsync<Reminder>(_options.ReminderCollection);
    }

    public async Task<IReadOnlyCollection<IReminder>> ListRemindersAsync(IUser user)
    {
        if (_options.ReminderCollection is null)
        {
            throw new NullReferenceException("Saved queue collection name not specified.");
        }
        
        return await _dataAccess.GetByUserAsync<Reminder>(_options.ReminderCollection, user);
    }

    public async Task DeleteReminderAsync(string id)
    {
        if (_options.ReminderCollection is null)
        {
            throw new NullReferenceException("Saved queue collection name not specified.");
        }
        
        await _dataAccess.DeleteAsync<Reminder>(_options.ReminderCollection, id);
    }
}