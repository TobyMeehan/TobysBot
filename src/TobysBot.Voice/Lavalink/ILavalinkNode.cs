using Discord;

namespace TobysBot.Voice.Lavalink;

public interface ILavalinkNode
{
    /// <summary>
    /// Starts a websocket connection with the lavalink node.
    /// </summary>
    /// <returns></returns>
    Task ConnectAsync();

    /// <summary>
    /// Disposes all players and closes the websocket connection.
    /// </summary>
    /// <returns></returns>
    Task DisconnectAsync();

    /// <summary>
    /// Gets the player for the specified guild.
    /// </summary>
    /// <param name="guild"></param>
    /// <returns></returns>
    ILavalinkPlayer GetPlayer(IGuild guild);

    /// <summary>
    /// Joins the specified voice channel, and binds the text channel if specified.
    /// </summary>
    /// <param name="voiceChannel"></param>
    /// <param name="textChannel"></param>
    /// <returns></returns>
    Task<ILavalinkPlayer> JoinAsync(IVoiceChannel voiceChannel, ITextChannel textChannel = null);

    /// <summary>
    /// Leaves the specified voice channel if connected.
    /// </summary>
    /// <param name="channel"></param>
    /// <returns></returns>
    Task LeaveAsync(IVoiceChannel channel);

    /// <summary>
    /// Moves the player to the specified voice channel.
    /// </summary>
    /// <param name="channel"></param>
    /// <returns></returns>
    Task MoveChannelAsync(IVoiceChannel channel);

    /// <summary>
    /// Rebinds player to the specified text channel.
    /// </summary>
    /// <param name="channel"></param>
    /// <returns></returns>
    Task RebindChannelAsync(ITextChannel channel);
}