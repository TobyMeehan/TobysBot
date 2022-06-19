using TobysBot.Commands;
using TobysBot.Events;
using TobysBot.Extensions;
using TobysBot.Music.Extensions;
using TobysBot.Voice;
using TobysBot.Voice.Events;
using TobysBot.Voice.Status;

namespace TobysBot.Music.Events;

public class TrackExceptionEventHandler : IEventHandler<SoundExceptionEventArgs>
{
    private readonly IMusicService _music;
    private readonly IVoiceService _voice;
    private readonly EmbedService _embeds;

    public TrackExceptionEventHandler(IMusicService music, IVoiceService voice, EmbedService embeds)
    {
        _music = music;
        _voice = voice;
        _embeds = embeds;
    }
    
    public async Task HandleAsync(SoundExceptionEventArgs args)
    {
        var track = await _music.GetTrackAsync(args.Guild);

        if (track.Position == TimeSpan.Zero)
        {
            if (args.Status is IConnectedStatus status)
            {
                await status.TextChannel.SendMessageAsync(embed: _embeds.Builder()
                    .WithContext(EmbedContext.Error)
                    .WithDescription($"Could not play [{track.Title}]({track.Url}), {args.Exception}")
                    .Build());
            }
            
            await _music.SkipAsync(args.Guild);
            
            return;
        }

        await _voice.PlayAsync(args.Guild, track.ToSound(), track.Position);
    }
}