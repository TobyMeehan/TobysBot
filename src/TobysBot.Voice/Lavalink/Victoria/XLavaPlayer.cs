using Discord;
using TobysBot.Voice.Effects;
using TobysBot.Voice.Extensions;
using TobysBot.Voice.Status;
using Victoria;
using Victoria.Enums;

namespace TobysBot.Voice.Lavalink.Victoria;

public class XLavaPlayer : LavaPlayer
{
    public XLavaPlayer(LavaSocket lavaSocket, IVoiceChannel voiceChannel, ITextChannel textChannel) : base(lavaSocket, voiceChannel, textChannel)
    {
        _playerPreset = new PlayerPreset();
        ActivePreset = _playerPreset;
    }
    
    private PlayerPreset _playerPreset;

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

    public IPreset ActivePreset { get; private set; }

    private async Task ApplyFiltersAsync()
    {
        await ApplyFiltersAsync(ActivePreset.GetLavaFilters(), Volume, ActivePreset.GetLavaEqualizer());
    }

    public async Task UpdateSpeedAsync(double speed)
    {
        _playerPreset.Speed = speed;
        ActivePreset = _playerPreset;

        await ApplyFiltersAsync();
    }

    public async Task UpdatePitchAsync(double pitch)
    {
        _playerPreset.Pitch = pitch;
        ActivePreset = _playerPreset;

        await ApplyFiltersAsync();
    }

    public async Task UpdateRotationAsync(double rotation)
    {
        _playerPreset.Rotation = rotation;
        ActivePreset = _playerPreset;

        await ApplyFiltersAsync();
    }

    public async Task UpdateEqualizerAsync(IEqualizer equalizer)
    {
        _playerPreset.Equalizer = new PlayerEqualizer(equalizer);
        ActivePreset = _playerPreset;

        await ApplyFiltersAsync();
    }
    
    public async Task SetActivePresetAsync(IPreset preset)
    {
        _playerPreset = new PlayerPreset(preset);
        ActivePreset = preset;

        await ApplyFiltersAsync();
    }

    public async Task RemoveActivePresetAsync()
    {
        await ResetEffectsAsync();
    }
    
    public async Task ResetEffectsAsync()
    {
        _playerPreset = new PlayerPreset();
        ActivePreset = _playerPreset;

        await ApplyFiltersAsync();
    }
}