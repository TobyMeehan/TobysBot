using Discord;
using TobysBot.Voice.Extensions;
using TobysBot.Voice.Status;
using Victoria;
using Victoria.Enums;

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
        await _lavaNode.JoinAsync(channel, textChannel);
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
    
    public async Task PlayAsync(ISound sound, IGuild guild)
    {
        var player = ThrowIfNoPlayer(guild);

        await player.PlayAsync(await _lavaNode.LoadSoundAsync(sound));
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

    public IPlayerStatus Status(IGuild guild)
    {
        if (_lavaNode.TryGetPlayer(guild, out var player))
        {
            return new NotConnectedStatus();
        }

        return player.Status;
    }
}