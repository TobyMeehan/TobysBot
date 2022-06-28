using Discord;
using TobysBot.Voice.Effects;
using TobysBot.Voice.Extensions;
using TobysBot.Voice.Status;
using Victoria;
using Victoria.Enums;
using Victoria.Filters;

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
        await ApplyFiltersAsync(GetLavaFilters(), Volume, GetLavaEqualizer());
    }

    private IEnumerable<IFilter> GetLavaFilters()
    {
        var result = new List<IFilter>();

        if (ActivePreset is not { Speed: 1, Pitch: 1 })
        {
            result.Add(new TimescaleFilter { Speed = ActivePreset.Speed, Pitch = ActivePreset.Pitch, Rate = 1 });
        }

        if (ActivePreset is not { Rotation: 0 })
        {
            result.Add(new RotationFilter { Hertz = ActivePreset.Rotation });
        }

        if (ActivePreset.ChannelMix is not { LeftToLeft: 1, LeftToRight: 0, RightToRight: 1, RightToLeft: 0 })
        {
            result.Add(new ChannelMixFilter
            {
                LeftToLeft = ActivePreset.ChannelMix.LeftToLeft, 
                LeftToRight = ActivePreset.ChannelMix.LeftToRight,
                RightToLeft = ActivePreset.ChannelMix.RightToLeft, 
                RightToRight = ActivePreset.ChannelMix.RightToRight
            });
        }

        return result;
    }

    private EqualizerBand[] GetLavaEqualizer()
    {
        if (ActivePreset.Equalizer.All(x => x.Gain == 0))
        {
            return Array.Empty<EqualizerBand>();
        }
        
        int i = 0;

        return ActivePreset.Equalizer.Select(x => new EqualizerBand(i++, x.Gain)).ToArray();
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

    public async Task UpdateChannelMixAsync(IChannelMix channelMix)
    {
        _playerPreset.ChannelMix = channelMix;
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