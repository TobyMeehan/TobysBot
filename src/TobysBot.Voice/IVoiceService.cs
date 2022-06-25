using Discord;
using TobysBot.Voice.Effects;
using TobysBot.Voice.Status;

namespace TobysBot.Voice;

/// <summary>
/// Service for playing sounds in the voice channel.
/// </summary>
public interface IVoiceService
{
    // Voice channel
    
    /// <summary>
    /// Joins the specified voice channel.
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="textChannel"></param>
    /// <returns></returns>
    Task JoinAsync(IVoiceChannel channel, ITextChannel? textChannel = null);

    /// <summary>
    /// Leaves the voice channel in the specified guild.
    /// </summary>
    /// <param name="guild"></param>
    /// <returns></returns>
    Task LeaveAsync(IGuild guild);

    /// <summary>
    /// Rebinds the text channel in the specified guild.
    /// </summary>
    /// <param name="textChannel"></param>
    /// <returns></returns>
    Task RebindChannelAsync(ITextChannel textChannel);
    
    // Player

    /// <summary>
    /// Plays the specified sound from the specified start time.
    /// </summary>
    /// <param name="guild"></param>
    /// <param name="sound"></param>
    /// <param name="startTime"></param>
    /// <returns></returns>
    Task PlayAsync(IGuild guild, ISound sound, TimeSpan startTime);

    /// <summary>
    /// Pauses the player in the specified guild.
    /// </summary>
    /// <param name="guild"></param>
    /// <returns></returns>
    Task PauseAsync(IGuild guild);

    /// <summary>
    /// Resumes the player in the specified guild.
    /// </summary>
    /// <param name="guild"></param>
    /// <returns></returns>
    Task ResumeAsync(IGuild guild);

    /// <summary>
    /// Seeks to the specified timespan in the specified guild.
    /// </summary>
    /// <param name="guild"></param>
    /// <param name="timeSpan"></param>
    /// <returns></returns>
    Task SeekAsync(IGuild guild, TimeSpan timeSpan);

    /// <summary>
    /// Stops the player in the specified guild.
    /// </summary>
    /// <param name="guild"></param>
    /// <returns></returns>
    Task StopAsync(IGuild guild);
    
    // Audio Effects

    /// <summary>
    /// Updates the player volume in the specified guild.
    /// </summary>
    /// <param name="guild"></param>
    /// <param name="volume"></param>
    /// <returns></returns>
    Task UpdateVolumeAsync(IGuild guild, ushort volume);
    
    /// <summary>
    /// Updates the speed in the specified guild.
    /// </summary>
    /// <param name="guild"></param>
    /// <param name="speed"></param>
    /// <returns></returns>
    Task UpdateSpeedAsync(IGuild guild, double speed);
    
    /// <summary>
    /// Updates the pitch in the specified guild.
    /// </summary>
    /// <param name="guild"></param>
    /// <param name="pitch"></param>
    /// <returns></returns>
    Task UpdatePitchAsync(IGuild guild, double pitch);
    
    /// <summary>
    /// Updates the rotation effect in the specified guild.
    /// </summary>
    /// <param name="guild"></param>
    /// <param name="hertz"></param>
    /// <returns></returns>
    Task UpdateRotationAsync(IGuild guild, double hertz);

    /// <summary>
    /// Updates the channel mix in the specified guild.
    /// </summary>
    /// <param name="guild"></param>
    /// <param name="channelMix"></param>
    /// <returns></returns>
    Task UpdateChannelMixAsync(IGuild guild, IChannelMix channelMix);

    /// <summary>
    /// Updates the equalizer in the specified guild.
    /// </summary>
    /// <param name="guild"></param>
    /// <param name="equalizer"></param>
    /// <returns></returns>
    Task UpdateEqualizerAsync(IGuild guild, IEqualizer equalizer);

    /// <summary>
    /// Gets the active effect preset for the specified guild.
    /// </summary>
    /// <param name="guild"></param>
    /// <returns></returns>
    Task<IPreset> GetActivePresetAsync(IGuild guild);
    
    /// <summary>
    /// Sets the active effect preset for the specified guild.
    /// </summary>
    /// <param name="guild"></param>
    /// <param name="preset"></param>
    /// <returns></returns>
    Task SetActivePresetAsync(IGuild guild, IPreset preset);
    
    /// <summary>
    /// Removes the active effect preset for the specified guild.
    /// </summary>
    /// <param name="guild"></param>
    /// <returns></returns>
    Task RemoveActivePresetAsync(IGuild guild);
    
    /// <summary>
    /// Resets all effects in the specified guild.
    /// </summary>
    /// <param name="guild"></param>
    /// <returns></returns>
    Task ResetEffectsAsync(IGuild guild);

    /// <summary>
    /// Gets the player status for the specified guild.
    /// </summary>
    /// <param name="guild"></param>
    /// <returns></returns>
    IPlayerStatus Status(IGuild guild);
}