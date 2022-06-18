using Discord;
using TobysBot.Voice.Effects;
using TobysBot.Voice.Extensions;
using TobysBot.Voice.Status;
using Victoria;
using Victoria.Enums;
using Victoria.Filters;

namespace TobysBot.Voice.Lavalink;

public class SoundPlayer : LavaPlayer
{
    public SoundPlayer(LavaSocket lavaSocket, IVoiceChannel voiceChannel, ITextChannel textChannel) : base(lavaSocket, voiceChannel, textChannel)
    {
        _playerPreset = new PlayerPreset();
        _activePreset = _playerPreset;
    }

    private PlayerPreset _playerPreset;
    private IPreset _activePreset;

    private async Task ApplyFiltersAsync()
    {
        await ApplyFiltersAsync(_activePreset.GetLavaFilters(), Volume, _activePreset.GetLavaEqualizer());
    }

    public async Task UpdateSpeedAsync(double speed)
    {
        _playerPreset.Speed = speed;

        await ApplyFiltersAsync();
    }

    public async Task UpdatePitchAsync(double pitch)
    {
        _playerPreset.Pitch = pitch;

        await ApplyFiltersAsync();
    }

    public async Task UpdateRotationAsync(double rotation)
    {
        _playerPreset.Rotation = rotation;

        await ApplyFiltersAsync();
    }

    public async Task UpdateEqualizerAsync(IEqualizer equalizer)
    {
        _playerPreset.Equalizer = new PlayerEqualizer(equalizer);

        await ApplyFiltersAsync();
    }

    public IPreset GetActivePreset()
    {
        return _activePreset;
    }
    
    public async Task SetActivePresetAsync(IPreset preset)
    {
        _activePreset = preset;

        await ApplyFiltersAsync();
    }

    public async Task RemoveActivePresetAsync()
    {
        _activePreset = _playerPreset;

        await ApplyFiltersAsync();
    }
    
    public async Task ResetEffectsAsync()
    {
        _playerPreset = new PlayerPreset();
        _activePreset = _playerPreset;

        await ApplyFiltersAsync();
    }

    public ISound Sound => new LavaSound(Track);

    public IPlayerStatus Status
    {
        get
        {
            if (!IsConnected)
            {
                return new NotConnectedStatus();
            }
            
            return PlayerState switch
            {
                PlayerState.Playing or PlayerState.Paused => new PlayingStatus(VoiceChannel, TextChannel,
                    Sound, Track.Position, PlayerState is PlayerState.Paused),
                _ => new NotPlayingStatus(VoiceChannel, TextChannel)
            };
        }
    }
}