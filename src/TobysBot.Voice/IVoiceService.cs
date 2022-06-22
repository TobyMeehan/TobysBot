using Discord;
using TobysBot.Voice.Effects;
using TobysBot.Voice.Status;

namespace TobysBot.Voice;

public interface IVoiceService
{
    // Voice channel
    
    Task JoinAsync(IVoiceChannel channel, ITextChannel? textChannel = null);

    Task LeaveAsync(IGuild guild);

    Task RebindChannelAsync(ITextChannel textChannel);
    
    // Player

    Task PlayAsync(IGuild guild, ISound sound, TimeSpan startTime);

    Task PauseAsync(IGuild guild);

    Task ResumeAsync(IGuild guild);

    Task SeekAsync(IGuild guild, TimeSpan timeSpan);

    Task StopAsync(IGuild guild);
    
    // Audio Effects

    Task UpdateVolumeAsync(IGuild guild, ushort volume);
    
    Task UpdateSpeedAsync(IGuild guild, double speed);
    Task UpdatePitchAsync(IGuild guild, double pitch);
    Task UpdateRotationAsync(IGuild guild, double hertz);

    Task UpdateEqualizerAsync(IGuild guild, IEqualizer equalizer);

    Task<IPreset> GetActivePresetAsync(IGuild guild);
    Task SetActivePresetAsync(IGuild guild, IPreset preset);
    Task RemoveActivePresetAsync(IGuild guild);
    
    Task ResetEffectsAsync(IGuild guild);

    IPlayerStatus Status(IGuild guild);
}