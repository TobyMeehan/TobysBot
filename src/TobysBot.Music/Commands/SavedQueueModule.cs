using Discord;
using Discord.Commands;
using TobysBot.Commands;
using TobysBot.Extensions;
using TobysBot.Music.Data;
using TobysBot.Music.Extensions;
using TobysBot.Voice;
using TobysBot.Voice.Commands;

namespace TobysBot.Music.Commands;

[Plugin("music")]
public class SavedQueueModule : VoiceCommandModuleBase
{
    private readonly ISavedQueueDataService _savedQueues;
    private readonly IMusicService _music;
    private readonly EmbedService _embeds;

    public SavedQueueModule(ISavedQueueDataService savedQueues, IMusicService music, IVoiceService voice,
        EmbedService embeds) : base(voice)
    {
        _savedQueues = savedQueues;
        _music = music;
        _embeds = embeds;
    }

    [Command("saved queues list", RunMode = RunMode.Async)]
    [Summary("Lists all of your saved queues.")]
    public async Task ListSavedQueuesAsync()
    {
        using var response = await Response.DeferAsync();
        
        var queues = await _savedQueues.ListSavedQueuesAsync(Context.User);

        if (!queues.Any())
        {
            await response.ModifyResponseAsync(x => x.Embed = _embeds.Builder()
                .WithContext(EmbedContext.Information)
                .WithDescription("You do not have any saved queues. Use **/saved queues create** to create one.")
                .Build());

            return;
        }

        await response.ModifyResponseAsync(x => x.Embed = _embeds.Builder()
            .WithSavedQueueListInformation(Context.User, queues)
            .Build());
    }

    [Command("saved queues create", RunMode = RunMode.Async)]
    [Summary("Saves the current queue under the specified name.")]
    [RequireContext(ContextType.Guild, ErrorMessage = "You must be in a guild to save a queue.")]
    [CheckVoice(sameChannel: SameChannel.Required)]
    public async Task CreateSavedQueueAsync(
        [Remainder]
        [Summary("Name of saved queue.")] string name)
    {
        if (name.HasSpecialCharacters())
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithContext(EmbedContext.Error)
                .WithDescription("Saved queue name cannot contain special characters.")
                .Build());
            
            return;
        }

        using var response = await Response.DeferAsync();
        
        var queue = await _music.GetQueueAsync(Context.Guild!);

        if (queue.Empty)
        {
            await response.ModifyResponseAsync(x => x.Embed = _embeds.Builder()
                .WithContext(EmbedContext.Error)
                .WithDescription("The queue is currently empty. Add some tracks to save it.")
                .Build());

            return;
        }

        await _savedQueues.CreateSavedQueueAsync(name, Context.User, queue);

        await response.ModifyResponseAsync(x => x.Embed = _embeds.Builder()
            .WithContext(EmbedContext.Action)
            .WithDescription($"{queue.Length} tracks saved to **{Format.Sanitize(name)}**")
            .Build());
    }

    [Command("saved queues delete", RunMode = RunMode.Async)]
    [Summary("Deletes the specified saved queue.")]
    public async Task DeleteSavedQueueAsync(
        [Remainder]
        [Summary("Name of queue to delete.")] string name)
    {
        using var response = await Response.DeferAsync();
        
        var savedQueue = await _savedQueues.GetSavedQueueAsync(Context.User, name, Context.User);

        if (savedQueue is null)
        {
            await response.ModifyResponseAsync(x => x.Embed = _embeds.Builder()
                .WithContext(EmbedContext.Error)
                .WithDescription("You have no saved queues with that name.")
                .Build());

            return;
        }

        await _savedQueues.DeleteSavedQueueAsync(Context.User, name);

        await response.ModifyResponseAsync(x => x.Embed = _embeds.Builder()
            .WithContext(EmbedContext.Action)
            .WithDescription($"Saved queue **{savedQueue.Name}** ({savedQueue.Tracks.Count()} tracks) deleted.")
            .Build());
    }

    [Command("saved queues share", RunMode = RunMode.Async)]
    [Summary("Gets a link that anyone can use to play your saved queue.")]
    public async Task ShareSavedQueueAsync(
        [Remainder]
        [Summary("Name of queue to share.")] string name)
    {
        using var response = await Response.DeferAsync();
        
        var savedQueue = await _savedQueues.GetSavedQueueAsync(Context.User, name, Context.User);

        if (savedQueue is null)
        {
            await response.ModifyResponseAsync(x => x.Embed = _embeds.Builder()
                .WithSavedQueueNotFoundError()
                .Build());

            return;
        }

        var link = _savedQueues.GetShareUri(savedQueue);

        await response.ModifyResponseAsync(x => x.Embed = _embeds.Builder()
            .WithContext(EmbedContext.Information)
            .WithDescription(
                $"Use the link {link.AbsoluteUri} to play your saved queue **{savedQueue.Name}** anywhere.")
            .Build());
    }

    [Command("saved queues load", RunMode = RunMode.Async)]
    [Summary("Loads the specified saved queue.")]
    [RequireContext(ContextType.Guild, ErrorMessage = "You must be in a guild to load a saved queue.")]
    [CheckVoice(sameChannel: SameChannel.IfBotConnected)]
    public async Task LoadSavedQueueAsync(
        [Remainder]
        [Summary("Name of queue to load.")] string name)
    {
        using var response = await Response.DeferAsync();

        var savedQueue = await _savedQueues.GetSavedQueueAsync(Context.User, name, Context.User);

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

        await _music.EnqueueAsync(Context.Guild!, savedQueue);

        await response.ModifyResponseAsync(x =>
        {
            x.Embed = _embeds.Builder()
                .WithQueueSavedQueueAction(savedQueue)
                .Build();
        });
    }
}