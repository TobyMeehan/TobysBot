using System.Text.RegularExpressions;
using Discord.Commands;
using Humanizer;
using Microsoft.Recognizers.Text;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using Microsoft.Recognizers.Text.DateTime;
using TobysBot.Commands;
using TobysBot.Extensions;
using TobysBot.Util.Data;

namespace TobysBot.Util.Commands;

[Plugin("util")]
public class ReminderModule : CommandModuleBase
{
    private readonly EmbedService _embeds;
    private readonly IReminderService _service;

    public ReminderModule(EmbedService embeds, IReminderService service)
    {
        _embeds = embeds;
        _service = service;
    }
    
    [Command("remindme create")]
    [Alias("remindme")]
    [Usage("remindme", "name] in [timestamp")]
    [Summary("Sets a reminder.")]
    public async Task RemindmeAsync(
        [Summary("Name of the reminder.")] string name,
        [Summary("Time until the reminder is activated.")] [Remainder]
        string timestamp)
    {
        var model = new DateTimeRecognizer().GetDateTimeModel();

        var results = model.Parse(timestamp);

        if (!results.Any())
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithContext(EmbedContext.Error)
                .WithDescription("Could not parse that timespan.")
                .Build());
            
            return;
        }

        var duration = results.FirstOrDefault(x => x.TypeName == "datetimeV2.duration");
        
        if (duration is null)
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithContext(EmbedContext.Error)
                .WithDescription("Could not parse that timespan.")
                .Build());
            
            return;
        }

        var values = duration.Resolution["values"] as List<Dictionary<string, string>>;

        string? seconds = values?.FirstOrDefault(x => x["type"] == "duration")?["value"];

        if (seconds is null)
        {
            await Response.ReplyAsync(embed: _embeds.Builder()
                .WithContext(EmbedContext.Error)
                .WithDescription("Could not parse that timespan.")
                .Build());
            
            return;
        }

        var timeSpan = TimeSpan.FromSeconds(double.Parse(seconds));
        var dateTime = DateTimeOffset.UtcNow + timeSpan;

        //await _service.CreateReminderAsync(Context.User, dateTime, name);

        await Response.ReplyAsync(embed: _embeds.Builder()
            .WithContext(EmbedContext.Action)
            .WithDescription($"A reminder has been set for **{timeSpan.Humanize()}** from now.")
            .Build());
    }

    [Command("remindme list")]
    [Summary("Lists your current reminders.")]
    public async Task RemindmeListAsync()
    {
        
    }
}