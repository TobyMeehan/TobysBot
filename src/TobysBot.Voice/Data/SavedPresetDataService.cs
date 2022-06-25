using Discord;
using Microsoft.Extensions.Options;
using TobysBot.Data;
using TobysBot.Voice.Configuration;
using TobysBot.Voice.Effects;

namespace TobysBot.Voice.Data;

public class SavedPresetDataService : ISavedPresetDataService
{
    private readonly IDataAccess _data;
    private readonly VoiceDataOptions _options;

    public SavedPresetDataService(IDataAccess data, IOptions<VoiceOptions> options)
    {
        _data = data;

        switch (options.Value.Data)
        {
            case null:
                throw new NullReferenceException("Voice data options not specified.");
            case {SavedPresetCollection: null}:
                throw new NullReferenceException("Saved preset collection name not specified.");
        }
        
        _options = options.Value.Data;
    }


    public async Task<IReadOnlyCollection<ISavedPreset>> ListSavedPresetsAsync(IUser user)
    {
        return await _data.GetByUserAsync<SavedPreset>(_options.SavedPresetCollection!, user);
    }

    public async Task<ISavedPreset?> GetSavedPresetAsync(string id)
    {
        return await _data.GetAsync<SavedPreset>(_options.SavedPresetCollection!, id);
    }

    public async Task<ISavedPreset?> GetSavedPresetAsync(IUser user, string name)
    {
        var result = await _data.GetByUserAsync<SavedPreset>(_options.SavedPresetCollection!, user, name);

        return result.FirstOrDefault();
    }

    public async Task CreateSavedPresetAsync(string name, IUser user, IPreset preset)
    {
        await _data.SaveByUserAsync(_options.SavedPresetCollection!, new SavedPreset(name, user.Id, preset));
    }

    public async Task DeleteSavedPresetAsync(IUser user, string name)
    {
        await _data.DeleteAsync<SavedPreset>(_options.SavedPresetCollection!, user, name);
    }
}