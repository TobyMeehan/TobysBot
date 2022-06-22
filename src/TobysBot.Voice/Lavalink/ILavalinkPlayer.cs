using Discord;
using TobysBot.Voice.Effects;
using TobysBot.Voice.Status;

namespace TobysBot.Voice.Lavalink;

public interface ILavalinkPlayer
{
    // Properties
    
    /// <summary>
    /// Voice channel this player is connected to.
    /// </summary>
    IVoiceChannel VoiceChannel { get; }
    
    /// <summary>
    /// Text channel this player is bound to.
    /// </summary>
    ITextChannel TextChannel { get; }
    
    /// <summary>
    /// Whether the player is connected to the voice gateway.
    /// </summary>
    bool IsConnected { get; }
    
    /// <summary>
    /// The currently playing sound.
    /// </summary>
    ISound Sound { get; }
    
    /// <summary>
    /// Current player status.
    /// </summary>
    IPlayerStatus Status { get; }
    
    /// <summary>
    /// Current effect preset.
    /// </summary>
    IPreset ActivePreset { get; }
    
    // Player
    
    /// <summary>
    /// Plays the specified sound, from the specified start time.
    /// </summary>
    /// <param name="sound"></param>
    /// <param name="startTime"></param>
    /// <returns></returns>
    Task PlayAsync(ISound sound, TimeSpan startTime);

    /// <summary>
    /// Pauses the current sound if any is playing.
    /// </summary>
    /// <returns></returns>
    Task PauseAsync();

    /// <summary>
    /// Resumes the current sound if any is playing.
    /// </summary>
    /// <returns></returns>
    Task ResumeAsync();

    /// <summary>
    /// Seeks the current sound to the specified position.
    /// </summary>
    /// <param name="timeSpan"></param>
    /// <returns></returns>
    Task SeekAsync(TimeSpan timeSpan);

    /// <summary>
    /// Stops the current track if any is playing.
    /// </summary>
    /// <returns></returns>
    Task StopAsync();

    /// <summary>
    /// Updates the current volume.
    /// </summary>
    /// <param name="volume"></param>
    /// <returns></returns>
    Task UpdateVolumeAsync(ushort volume);
    
    // Effects

    /// <summary>
    /// Updates the current speed.
    /// </summary>
    /// <param name="speed"></param>
    /// <returns></returns>
    Task UpdateSpeedAsync(double speed);

    /// <summary>
    /// Updates the current pitch.
    /// </summary>
    /// <param name="pitch"></param>
    /// <returns></returns>
    Task UpdatePitchAsync(double pitch);

    /// <summary>
    /// Updates the current rotation.
    /// </summary>
    /// <param name="hertz"></param>
    /// <returns></returns>
    Task UpdateRotationAsync(double hertz);

    /// <summary>
    /// Updates the current equalizer.
    /// </summary>
    /// <param name="equalizer"></param>
    /// <returns></returns>
    Task UpdateEqualizerAsync(IEqualizer equalizer);
    
    /// <summary>
    /// Resets all effects.
    /// </summary>
    /// <returns></returns>
    Task ResetEffectsAsync();

    // Effect Presets

    /// <summary>
    /// Sets the active preset.
    /// </summary>
    /// <param name="preset"></param>
    /// <returns></returns>
    Task SetActivePresetAsync(IPreset preset);

    /// <summary>
    /// Removes the current preset if any is set.
    /// </summary>
    /// <returns></returns>
    Task RemoveActivePresetAsync();
}