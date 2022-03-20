using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TobysBot.Discord.Audio.Extensions;
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

        public LavalinkAudioNode(LavaNode<XLavaPlayer> node, IQueue queue)
        {
            _node = node;
            _queue = queue;
            
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
            await _queue.ProgressAsync(arg.Player.VoiceChannel.GuildId, arg.Position);
        }

        private async Task NodeOnTrackStuck(TrackStuckEventArgs arg)
        {
            Console.WriteLine("Track stuck!");
            await arg.Player.PlayAsync(arg.Track);
        }

        private async Task NodeOnTrackException(TrackExceptionEventArgs arg)
        {
            Console.WriteLine(arg.ErrorMessage);

            var queue = await GetQueueAsync(arg.Player.VoiceChannel.Guild);
            
            await arg.Player.PlayAsync(await _node.LoadTrackAsync(queue.CurrentTrack));
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

            await arg.Player.PlayAsync(await _node.LoadTrackAsync(track));
        }

        private XLavaPlayer ThrowIfNoPlayer(IGuild guild)
        {
            if (!_node.TryGetPlayer(guild, out var player))
            {
                throw new Exception("No player is connected to the guild.");
            }

            return player;
        }

        public async Task JoinAsync(IVoiceChannel channel, ITextChannel textChannel = null)
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


        public async Task<ITrack> EnqueueAsync(IPlayable source, IGuild guild)
        {
            var player = ThrowIfNoPlayer(guild);

            SearchResponse search = await _node.SearchAsync(SearchType.Direct, source.Url);

            List<ITrack> tracks = new List<ITrack>();

            if (source is ITrack track)
            {
                if (search.Status != SearchStatus.TrackLoaded)
                {
                    throw new Exception(search.Exception.Message);
                }

                tracks.Add(track);
            }

            if (source is IPlaylist playlist)
            {
                if (search.Status != SearchStatus.PlaylistLoaded)
                {
                    throw new Exception(search.Exception.Message);
                }

                tracks.AddRange(playlist);
            }

            var playerState = player.PlayerState;
            
            if (playerState is not (PlayerState.Playing or PlayerState.Paused))
            {
                try
                {
                    await player.PlayAsync(search.Tracks.First());
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

        public async Task<ITrack> SkipAsync(IGuild guild, int index = 0)
        {
            var player = ThrowIfNoPlayer(guild);

            var queue = await GetQueueAsync(guild);

            if (queue.LoopEnabled is TrackLoopSetting)
            {
                await SetLoopAsync(guild, new DisabledLoopSetting());
            }

            ITrack nextTrack = await _queue.AdvanceAsync(guild.Id, index);

            if (nextTrack is null)
            {
                await player.StopAsync();
            }
            else
            {
                await player.PlayAsync(await _node.LoadTrackAsync(nextTrack));
            }
            
            return nextTrack;
        }

        public async Task ClearAsync(IGuild guild)
        {
            LavaPlayer player = ThrowIfNoPlayer(guild);

            await _queue.ClearAsync(guild.Id);

            await player.StopAsync();
        }

        public async Task StopAsync(IGuild guild)
        {
            LavaPlayer player = ThrowIfNoPlayer(guild);

            await _queue.ResetAsync(guild.Id);
            
            await player.StopAsync();
        }

        public IPlayerStatus Status(IGuild guild)
        {
            if (!_node.TryGetPlayer(guild, out var player))
            {
                return new NotPlayingStatus();
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
