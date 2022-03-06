using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;
using Victoria.Enums;
using Victoria.EventArgs;
using Victoria.Responses.Search;

namespace TobysBot.Discord.Audio.Lavalink
{
    public class LavalinkAudioNode : IAudioNode
    {
        private readonly LavaNode _node;
        private readonly IQueue _queue;

        public LavalinkAudioNode(LavaNode node, IQueue queue)
        {
            _node = node;
            _queue = queue;
            
            _node.OnTrackEnded += NodeOnTrackEnded;
            _node.OnTrackException += NodeOnTrackException;
            _node.OnTrackStuck += NodeOnTrackStuck;
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
            
            await arg.Player.PlayAsync(await LoadTrackAsync(queue.CurrentTrack));
        }

        private async Task NodeOnTrackEnded(TrackEndedEventArgs arg)
        {
            if (arg.Reason != TrackEndReason.Finished)
            {
                return;
            }

            var track = await _queue.AdvanceAsync(arg.Player.VoiceChannel.Guild);

            if (track is null)
            {
                return;
            }

            await arg.Player.PlayAsync(await LoadTrackAsync(track));
        }

        private LavaPlayer ThrowIfNoPlayer(IGuild guild)
        {
            if (!_node.TryGetPlayer(guild, out LavaPlayer player))
            {
                throw new Exception("No player is connected to the guild.");
            }

            return player;
        }

        private async Task<LavaTrack> LoadTrackAsync(ITrack track)
        {
            SearchResponse search = await _node.SearchAsync(SearchType.Direct, track.Url);

            if (search.Status != SearchStatus.TrackLoaded)
            {
                throw new Exception(search.Exception.Message);
            }

            return search.Tracks.First();
        }

        public async Task JoinAsync(IVoiceChannel channel, ITextChannel textChannel = null)
        {
            if (_node.TryGetPlayer(channel.Guild, out LavaPlayer player))
            {
                if (player.VoiceChannel != channel)
                {
                    await _node.MoveChannelAsync(channel);

                    textChannel ??= player.TextChannel;
                    
                    return;
                }
            }

            await _node.JoinAsync(channel, textChannel);
        }

        public async Task LeaveAsync(IGuild guild)
        {
            if (!_node.TryGetPlayer(guild, out LavaPlayer player))
            {
                return;
            }

            await _node.LeaveAsync(player.VoiceChannel);
        }


        public async Task<ITrack> EnqueueAsync(IPlayable source, IGuild guild)
        {
            LavaPlayer player = ThrowIfNoPlayer(guild);

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
            
            if (!player.HasTrack())
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
            
            await _queue.EnqueueAsync(guild, tracks);

            return new LavalinkTrack(player.Track);
        }

        public async Task PauseAsync(IGuild guild)
        {
            LavaPlayer player = ThrowIfNoPlayer(guild);

            await player.PauseAsync();
        }

        public async Task ResumeAsync(IGuild guild)
        {
            LavaPlayer player = ThrowIfNoPlayer(guild);

            await player.ResumeAsync();
        }

        public async Task<ITrack> SkipAsync(IGuild guild)
        {
            LavaPlayer player = ThrowIfNoPlayer(guild);

            ITrack nextTrack = await _queue.AdvanceAsync(guild);

            if (nextTrack is null)
            {
                await player.StopAsync();
            }
            else
            {
                await player.PlayAsync(await LoadTrackAsync(nextTrack));
            }
            
            return nextTrack;
        }

        public async Task ClearAsync(IGuild guild)
        {
            LavaPlayer player = ThrowIfNoPlayer(guild);

            await _queue.ClearAsync(guild);

            await player.StopAsync();
        }

        public async Task StopAsync(IGuild guild)
        {
            LavaPlayer player = ThrowIfNoPlayer(guild);

            await _queue.ResetAsync(guild);
            
            await player.StopAsync();
        }

        public IPlayerStatus Status(IGuild guild)
        {
            if (!_node.TryGetPlayer(guild, out LavaPlayer player))
            {
                return new NotPlayingStatus();
            }

            return player.PlayerState switch
            {
                PlayerState.Playing => new PlayingStatus(new LavalinkTrack(player.Track), player.Track.Position,
                    player.Track.Duration),
                PlayerState.Paused => new PausedStatus(new LavalinkTrack(player.Track), player.Track.Position,
                    player.Track.Duration),
                _ => new NotPlayingStatus()
            };
        }


        public Task<IVoiceChannel> GetCurrentChannelAsync(IGuild guild)
        {
            if (!_node.TryGetPlayer(guild, out LavaPlayer player))
            {
                return Task.FromResult<IVoiceChannel>(null);
            }

            return Task.FromResult(player.VoiceChannel);
        }

        public Task<ITrack> GetCurrentTrackAsync(IGuild guild)
        {
            if (!_node.TryGetPlayer(guild, out LavaPlayer player))
            {
                return Task.FromResult<ITrack>(null);
            }

            if (!player.HasTrack())
            {
                return Task.FromResult<ITrack>(null);
            }

            return Task.FromResult<ITrack>(new LavalinkTrack(player.Track));
        }

        public async Task<IQueueStatus> GetQueueAsync(IGuild guild)
        {
            return await _queue.GetAsync(guild);
        }
    }
}
