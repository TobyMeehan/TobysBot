using Discord;
using TobysBot.Voice.Extensions;
using TobysBot.Voice.Status;
using Victoria;
using Victoria.Enums;
using Victoria.Filters;

namespace TobysBot.Voice.Lavalink;

public class LavalinkVoiceService : IVoiceService
{
    private readonly LavaNode<SoundPlayer> _lavaNode;

    public LavalinkVoiceService(LavaNode<SoundPlayer> lavaNode)
    {
        _lavaNode = lavaNode;
    }
    
    // Voice channel

    private SoundPlayer ThrowIfNoPlayer(IGuild guild)
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

        await _lavaNode.MoveChannelAsync(textChannel);
    }

    // Player
    
    public async Task PlayAsync(IGuild guild, ISound sound, TimeSpan startTime)
    {
        var player = ThrowIfNoPlayer(guild);

        var track = await _lavaNode.LoadSoundAsync(sound);

        await player.PlayAsync(x =>
        {
            x.Track = track;
            x.StartTime = startTime;
        });
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

    public async Task UpdateBassBoostAsync(IGuild guild, double amount)
    {
        var player = ThrowIfNoPlayer(guild);

        int i = 0;

        var bands = new[]
        {
            0.2d,
            0.15d,
            0.1d,
            0.05d,
            0.0d,
            -0.005d,
            -0.01d,
            -0.01d,
            -0.01d,
            -0.01d,
            -0.01d,
            -0.01d,
            -0.01d,
            -0.01d,
            -0.01d
        }.Select(x => new EqualizerBand(i++, x * amount)).ToArray();

        await player.ApplyEqualizerAsync(bands);
    }

    public async Task UpdateRotationAsync(IGuild guild, double amount)
    {
        var player = ThrowIfNoPlayer(guild);

        await player.AddFilterAsync(new RotationFilter { Hertz = amount });
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