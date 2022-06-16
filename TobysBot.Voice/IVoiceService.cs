using Discord;
using TobysBot.Voice.Effects;
using TobysBot.Voice.Status;

namespace TobysBot.Voice;

public interface IVoiceService
{
    // Voice channel
    
    Task JoinAsync(IVoiceChannel channel, ITextChannel textChannel = null);

    Task LeaveAsync(IGuild guild);

    Task RebindChannelAsync(ITextChannel textChannel);
    
    // Player

    Task PlayAsync(IGuild guild, ISound sound, TimeSpan startTime);

    Task PauseAsync(IGuild guild);

    Task ResumeAsync(IGuild guild);

    Task SeekAsync(IGuild guild, TimeSpan timeSpan);

    Task StopAsync(IGuild guild);
    
    // Audio Effects

    Task SetEffectAsync(IGuild guild, IEffect effect);

    IPlayerStatus Status(IGuild guild);
}