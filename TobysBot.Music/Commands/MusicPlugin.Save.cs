using Discord;
using Discord.Commands;
using TobysBot.Commands;
using TobysBot.Extensions;
using TobysBot.Music.Data;
using TobysBot.Music.Extensions;
using TobysBot.Voice.Commands;

namespace TobysBot.Music.Commands;

public partial class MusicPlugin
{
    public class SaveModule : CommandModuleBase
    {
        private readonly ISavedQueueDataService _savedQueues;
        private readonly IMusicService _music;
        private readonly EmbedService _embeds;

        public SaveModule(ISavedQueueDataService savedQueues, IMusicService music, EmbedService embeds)
        {
            _savedQueues = savedQueues;
            _music = music;
            _embeds = embeds;
        }
        
        [Command("saved queues list")]
        [Summary("Lists all of your saved queues.")]
        public async Task ListSavedQueuesAsync()
        {
            var queues = await _savedQueues.ListSavedQueuesAsync(Context.User);

            if (!queues.Any())
            {
                await Response.ReplyAsync(embed: _embeds.Builder()
                    .WithContext(EmbedContext.Information)
                    .WithDescription("You do not have any saved queues. Use **/saved queues create** to create one.")
                    .Build());
                
                return;
            }

            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithSavedQueueListInformation(Context.User, queues)
                .Build());
        }

        [Command("saved queues create")]
        [Summary("Saves the current queue under the specified name.")]
        [CheckVoice(sameChannel: SameChannel.Required)]
        public async Task CreateSavedQueueAsync(
            [Summary("Name of saved queue.")] string name)
        {
            var queue = await _music.GetQueueAsync(Context.Guild);

            if (queue.Empty)
            {
                await Response.ReplyAsync(embed: _embeds.Builder()
                    .WithContext(EmbedContext.Error)
                    .WithDescription("The queue is currently empty. Add some tracks to save it.")
                    .Build());
                
                return;
            }

            await _savedQueues.CreateSavedQueueAsync(name, Context.User, queue);

            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithContext(EmbedContext.Action)
                .WithDescription($"{queue.Length} tracks saved to **{Format.Sanitize(name)}**")
                .Build());
        }

        [Command("saved queues delete")]
        [Summary("Deletes the specified saved queue.")]
        public async Task DeleteSavedQueueAsync(
            [Summary("Name of queue to delete.")] string name)
        {
            var savedQueue = await _savedQueues.GetSavedQueueAsync(Context.User, name);

            if (savedQueue is null)
            {
                await Response.ReplyAsync(embed: _embeds.Builder()
                    .WithContext(EmbedContext.Error)
                    .WithDescription("You have no saved queues with that name.")
                    .Build());
                
                return;
            }

            await _savedQueues.DeleteSavedQueueAsync(Context.User, name);

            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithContext(EmbedContext.Action)
                .WithDescription($"**{savedQueue.Name}** ({savedQueue.Tracks.Count()} tracks) deleted.")
                .Build());
        }
    }
}