using TobysBot.Commands;
using TobysBot.Events;
using TobysBot.Music.Extensions;
using TobysBot.Voice.Events;

namespace TobysBot.Music.Events;

public class TrackNotificationEventHandler : IEventHandler<SoundStartedEventArgs>
{
    private readonly IMusicService _music;
    private readonly EmbedService _embeds;

    public TrackNotificationEventHandler(IMusicService music, EmbedService embeds)
    {
        _music = music;
        _embeds = embeds;
    }
    
    public async Task HandleAsync(SoundStartedEventArgs args)
    {
        var track = await _music.GetTrackAsync(args.Guild);

        if (track is null)
        {
            return;
        }

        await args.TextChannel.SendMessageAsync(embed: _embeds.Builder()
            .WithPlayTrackAction(track)
            .Build());
    }
}