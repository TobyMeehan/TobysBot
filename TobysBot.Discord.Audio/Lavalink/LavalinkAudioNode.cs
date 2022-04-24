using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TobysBot.Discord.Audio.Extensions;
using TobysBot.Discord.Audio.Status;
using Victoria;
using Victoria.Enums;
using Victoria.EventArgs;
using Victoria.Responses.Search;

namespace TobysBot.Discord.Audio.Lavalink
{
    public class LavalinkAudioNode : IAudioNode
    {
        private readonly LavaNode<XLavaPlayer> _node;
        private readonly IQueue _queue;
        private readonly ILogger<LavalinkAudioNode> _logger;

        public LavalinkAudioNode(LavaNode<XLavaPlayer> node, IQueue queue, ILogger<LavalinkAudioNode> logger)
        {
            _node = node;
            _queue = queue;
            _logger = logger;

            _node.OnTrackEnded += NodeOnTrackEnded;
            _node.OnTrackException += NodeOnTrackException;
            _node.OnTrackStuck += NodeOnTrackStuck;
            _node.OnPlayerUpdated += NodeOnPlayerUpdated;
            _node.OnWebSocketClosed += NodeOnWebSocketClosed;
        }

        private Task NodeOnWebSocketClosed(WebSocketClosedEventArgs arg)
        {
            return Task.CompletedTask;
        }

        private async Task NodeOnPlayerUpdated(PlayerUpdateEventArgs arg)
        {
            if (arg.Position.HasValue)
            {
                await _queue.ProgressAsync(arg.Player.VoiceChannel.GuildId, arg.Position.Value);
            }
        }

        private async Task NodeOnTrackStuck(TrackStuckEventArgs arg)
        {
            _logger.LogError("Track {Track} stuck. \n" +
                             "\tPlayer Guild: {Player} \n" +
                             "\tAt Threshold: {Threshold}",
                arg.Track.Title, arg.Player.VoiceChannel.GuildId, arg.Threshold);

            await SkipAsync(arg.Player.VoiceChannel.Guild);
        }

        private async Task NodeOnTrackException(TrackExceptionEventArgs arg)
        {
            _logger.LogError("Exception thrown in track: {Track} \n" +
                             "\tPlayer Guild: {Player} \n" +
                             "\tException: {Message}",
                arg.Track.Title, arg.Player.VoiceChannel.GuildId, arg.Exception);

            var queue = await GetQueueAsync(arg.Player.VoiceChannel.Guild);

            await SkipAsync(arg.Player.VoiceChannel.Guild);
        }

        private async Task NodeOnTrackEnded(TrackEndedEventArgs arg)
        {
            if (arg.Reason != TrackEndReason.Finished)
            {
                return;
            }

            var track = await _queue.AdvanceAsync(arg.Player.VoiceChannel.GuildId);

            if (track is null)
            {
                return;
            }

            if (arg.Player is not XLavaPlayer player)
            {
                return;
            }
            
            await player.PlayAsync(await _node.LoadTrackAsync(track), track.Title, track.Author);
        }

        private XLavaPlayer ThrowIfNoPlayer(IGuild guild)
        {
            if (!_node.TryGetPlayer(guild, out var player))
            {
                throw new Exception("No player is connected to the guild.");
            }

            return player;
        }

        public async Task JoinAsync(IVoiceChannel channel, ITextChannel textChannel)
        {
            if (_node.TryGetPlayer(channel.Guild, out var player))
            {
                if (player.VoiceChannel == channel)
                {
                    return;
                }
                
                await _node.MoveChannelAsync(channel);
                    
                return;

            }

            await _node.JoinAsync(channel, textChannel);
        }

        public async Task LeaveAsync(IGuild guild)
        {
            if (!_node.TryGetPlayer(guild, out var player))
            {
                return;
            }

            await _node.LeaveAsync(player.VoiceChannel);
            await _queue.ClearAsync(guild.Id);
        }

        public async Task RebindChannelAsync(ITextChannel textChannel)
        {
            ThrowIfNoPlayer(textChannel.Guild);

            await _node.MoveChannelAsync(textChannel);
        }


        public async Task<ITrack> EnqueueAsync(IPlayable source, IGuild guild)
        {
            var player = ThrowIfNoPlayer(guild);

            var tracks = new List<ITrack>();

            if (source is ITrack track)
            {
                tracks.Add(track);
            }

            if (source is IPlaylist playlist)
            {
                tracks.AddRange(playlist);
            }

            var playerState = player.PlayerState;
            
            if (playerState is not (PlayerState.Playing or PlayerState.Paused))
            {
                try
                {
                    var trackToPlay = tracks.First();
                    await player.PlayAsync(await _node.LoadTrackAsync(trackToPlay), trackToPlay.Title, trackToPlay.Author);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            
            await _queue.EnqueueAsync(guild.Id, tracks, advanceToTracks: playerState is PlayerState.Stopped);

            return player.CurrentTrack;
        }

        public async Task PauseAsync(IGuild guild)
        {
            LavaPlayer player = ThrowIfNoPlayer(guild);

            await player.PauseAsync();
        }

        public async Task ResumeAsync(IGuild guild)
        {
            LavaPlayer player = ThrowIfNoPlayer(guild);

            if (player.PlayerState is PlayerState.Paused)
            {
                await player.ResumeAsync();
            }

            if (player.PlayerState is PlayerState.Stopped)
            {
                var queue = await GetQueueAsync(guild);

                var track = queue?.CurrentTrack;

                if (track is null)
                {
                    return;
                }
                
                await player.PlayAsync(await _node.LoadTrackAsync(track));
            }
            
        }

        public async Task SeekAsync(IGuild guild, TimeSpan timeSpan)
        {
            LavaPlayer player = ThrowIfNoPlayer(guild);

            try
            {
                await player.SeekAsync(timeSpan);
            }
            catch
            {
                throw;
            }
        }

        public async Task<ITrack> SkipAsync(IGuild guild)
        {
            var player = ThrowIfNoPlayer(guild);

            var nextTrack = await _queue.AdvanceAsync(guild.Id, true);

            if (nextTrack is null)
            {
                await player.StopAsync();
            }
            else
            {
                await player.PlayAsync(await _node.LoadTrackAsync(nextTrack), nextTrack.Title, nextTrack.Author);
            }
            
            return nextTrack;
        }

        public async Task<ITrack> BackAsync(IGuild guild)
        {
            var player = ThrowIfNoPlayer(guild);

            var previousTrack = await _queue.BackAsync(guild.Id);

            if (previousTrack is null)
            {
                throw new Exception("No previous track to play.");
            }

            await player.PlayAsync(await _node.LoadTrackAsync(previousTrack));

            return previousTrack;
        }

        public async Task<ITrack> JumpAsync(IGuild guild, int index)
        {
            var player = ThrowIfNoPlayer(guild);

            if (index < 1)
            {
                throw new ArgumentException("Index must be > 1", nameof(index));
            }
            
            var track = await _queue.JumpAsync(guild.Id, index);

            if (track is null)
            {
                throw new Exception("Invalid track or index.");
            }

            await player.PlayAsync(await _node.LoadTrackAsync(track));

            return track;
        }

        public async Task ClearAsync(IGuild guild)
        {
            var player = ThrowIfNoPlayer(guild);

            await _queue.ClearAsync(guild.Id);

            await player.StopAsync();
        }

        public async Task StopAsync(IGuild guild)
        {
            var player = ThrowIfNoPlayer(guild);

            await _queue.ResetAsync(guild.Id);
            
            await player.StopAsync();
        }

        public async Task RemoveAsync(IGuild guild, int position)
        {
            var player = ThrowIfNoPlayer(guild);
            
            var (trackChanged, currentTrack) = await _queue.RemoveAsync(guild.Id, position);

            if (trackChanged)
            {
                await player.PlayAsync(await _node.LoadTrackAsync(currentTrack));
            }
        }

        public async Task RemoveRangeAsync(IGuild guild, int startTrack, int endTrack)
        {
            var player = ThrowIfNoPlayer(guild);
            
            var (trackChanged, currentTrack) = await _queue.RemoveRangeAsync(guild.Id, startTrack, endTrack);

            if (trackChanged)
            {
                await player.PlayAsync(await _node.LoadTrackAsync(currentTrack));
            }
        }

        public async Task MoveAsync(IGuild guild, int track, int newPos)
        {
            var player = ThrowIfNoPlayer(guild);
            
            var (trackChanged, currentTrack) = await _queue.MoveAsync(guild.Id, track, newPos);

            if (trackChanged)
            {
                await player.PlayAsync(await _node.LoadTrackAsync(currentTrack));
            }
        }

        public IPlayerStatus Status(IGuild guild)
        {
            if (!_node.TryGetPlayer(guild, out var player))
            {
                return new NotConnectedStatus();
            }

            return player.Status;
        }


        public Task<IVoiceChannel> GetCurrentChannelAsync(IGuild guild)
        {
            if (!_node.TryGetPlayer(guild, out var player))
            {
                return Task.FromResult<IVoiceChannel>(null);
            }

            return Task.FromResult(player.VoiceChannel);
        }

        public Task<ITrack> GetCurrentTrackAsync(IGuild guild)
        {
            if (!_node.TryGetPlayer(guild, out var player))
            {
                return Task.FromResult<ITrack>(null);
            }

            if (!player.HasTrack())
            {
                return Task.FromResult<ITrack>(null);
            }

            return Task.FromResult(player.CurrentTrack);
        }

        public async Task<IQueueStatus> GetQueueAsync(IGuild guild)
        {
            return await _queue.GetAsync(guild.Id);
        }

        public Task SetLoopAsync(IGuild guild, LoopSetting setting)
        {
            return _queue.SetLoopAsync(guild.Id, setting);
        }

        public Task SetShuffleAsync(IGuild guild, ShuffleSetting setting)
        {
            return _queue.SetShuffleAsync(guild.Id, setting);
        }

        public Task ShuffleAsync(IGuild guild)
        {
            return _queue.ShuffleAsync(guild.Id);
        }
    }
}
