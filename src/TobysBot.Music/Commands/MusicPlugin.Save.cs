﻿using Discord;
using Discord.Commands;
using TobysBot.Commands;
using TobysBot.Extensions;
using TobysBot.Music.Data;
using TobysBot.Music.Extensions;
using TobysBot.Voice;
using TobysBot.Voice.Commands;

namespace TobysBot.Music.Commands;

public partial class MusicPlugin
{
    public class SaveModule : VoiceCommandModuleBase
    {
        private readonly ISavedQueueDataService _savedQueues;
        private readonly IMusicService _music;
        private readonly ISearchService _search;
        private readonly EmbedService _embeds;

        public SaveModule(ISavedQueueDataService savedQueues, IMusicService music, ISearchService search, IVoiceService voice, EmbedService embeds) : base(voice, embeds)
        {
            _savedQueues = savedQueues;
            _music = music;
            _search = search;
            _embeds = embeds;
        }
        
        [Command("queues list")]
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

        [Command("queues create")]
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

        [Command("queues delete")]
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
                .WithDescription($"Saved queue **{savedQueue.Name}** ({savedQueue.Tracks.Count()} tracks) deleted.")
                .Build());
        }

        [Command("queues share")]
        [Summary("Gets a link that anyone can use to play your saved queue.")]
        public async Task ShareSavedQueueAsync(
            [Summary("Name of queue to share.")] string name)
        {
            var savedQueue = await _savedQueues.GetSavedQueueAsync(Context.User, name);

            if (savedQueue is null)
            {
                await Response.ReplyAsync(embed: _embeds.Builder()
                    .WithSavedQueueNotFoundError()
                    .Build());
                
                return;
            }

            var link = _savedQueues.GetShareUri(savedQueue);

            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithContext(EmbedContext.Information)
                .WithDescription(
                    $"Use the link {link.AbsoluteUri} to play your saved queue **{savedQueue.Name}** anywhere.")
                .Build());
        }

        [Command("queues load", RunMode = RunMode.Async)]
        [Summary("Loads the specified saved queue.")]
        [CheckVoice(sameChannel: SameChannel.IfBotConnected)]
        public async Task LoadSavedQueueAsync(
            [Summary("Name of queue to load.")] string name)
        {
            using var response = await Response.DeferAsync();
            
            var savedQueue = await _savedQueues.GetSavedQueueAsync(Context.User, name);
            
            if (savedQueue is null)
            {
                await response.ModifyResponseAsync(x =>
                {
                    x.Embed = _embeds.Builder()
                        .WithSavedQueueNotFoundError()
                        .Build();
                });
                
                return;
            }

            await JoinVoiceChannelAsync();

            await _music.EnqueueAsync(Context.Guild, savedQueue);

            await response.ModifyResponseAsync(x =>
            {
                x.Embed = _embeds.Builder()
                    .WithQueueSavedQueueAction(savedQueue)
                    .Build();
            });
        }
    }
}