using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;
using Victoria.Enums;
using Victoria.Responses.Search;

namespace TobysBot.Discord.Audio.Lavalink
{
    public class LavalinkAudioNode : IAudioNode
    {
        private readonly LavaNode _node;

        public LavalinkAudioNode(LavaNode node)
        {
            _node = node;
        }

        // Voice channel
        public async Task JoinAsync(IVoiceChannel channel, ITextChannel textChannel)
        {
            IVoiceChannel currentChannel = await GetCurrentChannelAsync(channel.Guild);

            if (currentChannel?.Id != channel.Id)
            {
                await _node.LeaveAsync(currentChannel);
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

        // Track controls
        public async Task<ITrack> PlayAsync(string query, IVoiceChannel channel, ITextChannel textChannel)
        {
            await JoinAsync(channel, textChannel);

            LavaPlayer player = _node.GetPlayer(channel.Guild);

            var search = Uri.IsWellFormedUriString(query, UriKind.Absolute) ?
                    await _node.SearchAsync(SearchType.Direct, query)
                    : await _node.SearchYouTubeAsync(query);

            if (search.Status == SearchStatus.NoMatches)
            {
                throw new Exception("No matches for query.");
            }

            LavaTrack track = search.Tracks.FirstOrDefault();

            if (player.IsPlaying()) // TODO: use better queue
            {
                player.Queue.Enqueue(track);
            }
            else
            {
                await player.PlayAsync(track);
            }

            return new LavalinkTrack(track);
        }

        public async Task PauseAsync(IGuild guild)
        {
            if (!_node.TryGetPlayer(guild, out LavaPlayer player))
            {
                return;
            }

            await player.PauseAsync();
        }

        public async Task StopAsync(IGuild guild)
        {
            if (!_node.TryGetPlayer(guild, out LavaPlayer player))
            {
                return;
            }

            await player.StopAsync();

            // TODO: go to start of queue
        }

        // Queue
        public Task<ITrack> SkipAsync(IGuild guild)
        {
            throw new NotImplementedException();
        }

        public Task ClearAsync(IGuild guild)
        {
            throw new NotImplementedException();
        }


        // Information
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

            if (!player.IsPlaying())
            {
                return Task.FromResult<ITrack>(null);
            }

            return Task.FromResult<ITrack>(new LavalinkTrack(player.Track));
        }

        public Task<IQueue> GetQueueAsync(IGuild guild)
        {
            throw new NotImplementedException();
        }
    }
}
