using Discord;
using TobysBot.Voice.Effects;
using TobysBot.Voice.Extensions;
using TobysBot.Voice.Status;
using Victoria;

namespace TobysBot.Voice.Lavalink.Victoria;

public class VictoriaLavalinkPlayer : ILavalinkPlayer
{
    private readonly XLavaPlayer _player;
    private readonly LavaNode<XLavaPlayer> _node;

    public VictoriaLavalinkPlayer(XLavaPlayer player, LavaNode<XLavaPlayer> node)
    {
        _player = player;
        _node = node;
    }

    public IVoiceChannel VoiceChannel => _player.VoiceChannel;
    public ITextChannel TextChannel => _player.TextChannel;
    public bool IsConnected => _player.IsConnected;
    public ISound Sound => _player.Sound;
    public IPlayerStatus Status => _player.Status;
    public IPreset ActivePreset => _player.ActivePreset;
    
    public async Task PlayAsync(ISound sound, TimeSpan startTime)
    {
        var track = await _node.LoadSoundAsync(sound);

        await _player.PlayAsync(x =>
        {
            x.Track = track;
            x.StartTime = startTime;
        });
    }

    public async Task PauseAsync()
    {
        await _player.PauseAsync();
    }

    public async Task ResumeAsync()
    {
        await _player.ResumeAsync();
    }

    public async Task SeekAsync(TimeSpan timeSpan)
    {
        await _player.SeekAsync(timeSpan);
    }

    public async Task StopAsync()
    {
        await _player.StopAsync();
    }

    public async Task UpdateVolumeAsync(ushort volume)
    {
        await _player.UpdateVolumeAsync(volume);
    }

    public async Task UpdateSpeedAsync(double speed)
    {
        await _player.UpdateSpeedAsync(speed);
    }

    public async Task UpdatePitchAsync(double pitch)
    {
        await _player.UpdatePitchAsync(pitch);
    }

    public async Task UpdateRotationAsync(double hertz)
    {
        await _player.UpdateRotationAsync(hertz);
    }

    public async Task UpdateChannelMixAsync(IChannelMix channelMix)
    {
        await _player.UpdateChannelMixAsync(channelMix);
    }

    public async Task UpdateEqualizerAsync(IEqualizer equalizer)
    {
        await _player.UpdateEqualizerAsync(equalizer);
    }

    public async Task ResetEffectsAsync()
    {
        await _player.ResetEffectsAsync();
    }

    public async Task SetActivePresetAsync(IPreset preset)
    {
        await _player.SetActivePresetAsync(preset);
    }

    public async Task RemoveActivePresetAsync()
    {
        await _player.RemoveActivePresetAsync();
    }
}