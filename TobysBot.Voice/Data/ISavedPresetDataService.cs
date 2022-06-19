using Discord;
using TobysBot.Voice.Effects;

namespace TobysBot.Voice.Data;

public interface ISavedPresetDataService
{
    Task<IReadOnlyCollection<ISavedPreset>> ListSavedPresetsAsync(IUser user);
    Task<ISavedPreset> GetSavedPresetAsync(string id);
    Task<ISavedPreset> GetSavedPresetAsync(IUser user, string name);
    Task CreateSavedPresetAsync(string name, IUser user, IPreset preset);
    Task DeleteSavedPresetAsync(IUser user, string name);
}