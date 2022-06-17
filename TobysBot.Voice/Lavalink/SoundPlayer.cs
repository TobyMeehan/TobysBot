using Discord;
using TobysBot.Voice.Status;
using Victoria;
using Victoria.Enums;
using Victoria.Filters;

namespace TobysBot.Voice.Lavalink;

public class SoundPlayer : LavaPlayer
{
    public SoundPlayer(LavaSocket lavaSocket, IVoiceChannel voiceChannel, ITextChannel textChannel) : base(lavaSocket, voiceChannel, textChannel)
    {
    }

    private List<IFilter> _filters = new() { new TimescaleFilter {Speed = 1, Pitch = 1, Rate = 1} };

    public async Task UpdateSpeedAsync(double speed)
    {
        var activeFilter = _filters.OfType<TimescaleFilter>().FirstOrDefault();

        _filters.Remove(activeFilter);
        
        _filters.Add(new TimescaleFilter{Speed = speed, Pitch = activeFilter.Pitch, Rate = activeFilter.Rate});

        await ApplyFiltersAsync(_filters, Volume, Equalizer.ToArray());
    }

    public async Task UpdatePitchAsync(double pitch)
    {
        var activeFilter = _filters.OfType<TimescaleFilter>().FirstOrDefault();

        _filters.Remove(activeFilter);
        
        _filters.Add(new TimescaleFilter{Pitch = pitch, Speed = activeFilter.Speed, Rate = activeFilter.Rate});

        await ApplyFiltersAsync(_filters, Volume, Equalizer.ToArray());
    }

    public async Task AddFilterAsync<TFilter>(TFilter filter) where TFilter : IFilter
    {
        var activeFilter = _filters.OfType<TFilter>().FirstOrDefault();

        _filters.Remove(activeFilter);
        
        _filters.Add(filter);

        await ApplyFiltersAsync(_filters, Volume, Equalizer.ToArray());
    }

    public async Task RemoveFilterAsync<TFilter>() where TFilter : IFilter
    {
        var filter = _filters.OfType<TFilter>().FirstOrDefault();

        if (filter is not null)
        {
            _filters.Remove(filter);

            await ApplyFiltersAsync(_filters, Volume, Equalizer.ToArray());
        }
    }

    public async Task ApplyEqualizerAsync(EqualizerBand[] equalizer)
    {
        await ApplyFiltersAsync(_filters, Volume, equalizer);
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