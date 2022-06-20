using Discord;
using TobysBot.Voice.Effects;
using TobysBot.Voice.Extensions;
using TobysBot.Voice.Status;
using Victoria;
using Victoria.Enums;
using Victoria.Filters;

namespace TobysBot.Voice.Lavalink;

public class LavalinkVoiceService : IVoiceService
{
    private readonly ILavalinkNode _lavaNode;

    public LavalinkVoiceService(ILavalinkNode lavaNode)
    {
        _lavaNode = lavaNode;
    }
    
    // Voice channel

    private ILavalinkPlayer ThrowIfNoPlayer(IGuild guild)
    {
        if (!_lavaNode.TryGetPlayer(guild, out var player))
        {
            throw new Exception("No player is connected to the guild.");
        }

        return player;
    }
    
    public async Task JoinAsync(IVoiceChannel channel, ITextChannel textChannel = null)
    {
        if (!_lavaNode.TryGetPlayer(channel.Guild, out var player) || !player.IsConnected)
        {
            await _lavaNode.JoinAsync(channel, textChannel);
            return;
        }
        
        if (player.VoiceChannel?.Id == channel.Id)
        {
            return;
        }

        await _lavaNode.MoveChannelAsync(channel);
    }

    public async Task LeaveAsync(IGuild guild)
    {
        if (!_lavaNode.TryGetPlayer(guild, out var player))
        {
            return;
        }

        await _lavaNode.LeaveAsync(player.VoiceChannel);
    }

    public async Task RebindChannelAsync(ITextChannel textChannel)
    {
        ThrowIfNoPlayer(textChannel.Guild);

        await _lavaNode.RebindChannelAsync(textChannel);
    }

    // Player
    
    public async Task PlayAsync(IGuild guild, ISound sound, TimeSpan startTime)
    {
        var player = ThrowIfNoPlayer(guild);

        await player.PlayAsync(sound, startTime);
    }

    public async Task PauseAsync(IGuild guild)
    {
        var player = ThrowIfNoPlayer(guild);

        await player.PauseAsync();
    }

    public async Task ResumeAsync(IGuild guild)
    {
        var player = ThrowIfNoPlayer(guild);

        await player.ResumeAsync();
    }

    public async Task SeekAsync(IGuild guild, TimeSpan timeSpan)
    {
        var player = ThrowIfNoPlayer(guild);

        await player.SeekAsync(timeSpan);
    }

    public async Task StopAsync(IGuild guild)
    {
        var player = ThrowIfNoPlayer(guild);

        await player.StopAsync();
    }

    public async Task UpdateVolumeAsync(IGuild guild, ushort volume)
    {
        var player = ThrowIfNoPlayer(guild);
        
        await player.UpdateVolumeAsync(volume);
    }

    public async Task UpdateSpeedAsync(IGuild guild, double speed)
    {
        var player = ThrowIfNoPlayer(guild);

        await player.UpdateSpeedAsync(speed);
    }

    public async Task UpdatePitchAsync(IGuild guild, double pitch)
    {
        var player = ThrowIfNoPlayer(guild);

        await player.UpdatePitchAsync(pitch);
    }

    public async Task UpdateRotationAsync(IGuild guild, double hertz)
    {
        var player = ThrowIfNoPlayer(guild);

        await player.UpdateRotationAsync(hertz);
    }

    public async Task UpdateEqualizerAsync(IGuild guild, IEqualizer equalizer)
    {
        var player = ThrowIfNoPlayer(guild);

        await player.UpdateEqualizerAsync(equalizer);
    }

    public Task<IPreset> GetActivePresetAsync(IGuild guild)
    {
        var player = ThrowIfNoPlayer(guild);

        return Task.FromResult(player.ActivePreset);
    }

    public async Task SetActivePresetAsync(IGuild guild, IPreset preset)
    {
        var player = ThrowIfNoPlayer(guild);

        await player.SetActivePresetAsync(preset);
    }

    public async Task RemoveActivePresetAsync(IGuild guild)
    {
        var player = ThrowIfNoPlayer(guild);

        await player.RemoveActivePresetAsync();
    }

    public async Task ResetEffectsAsync(IGuild guild)
    {
        var player = ThrowIfNoPlayer(guild);

        await player.ResetEffectsAsync();
    }

    public IPlayerStatus Status(IGuild guild)
    {
        if (!_lavaNode.TryGetPlayer(guild, out var player))
        {
            return new NotConnectedStatus();
        }

        return player.Status;
    }
}