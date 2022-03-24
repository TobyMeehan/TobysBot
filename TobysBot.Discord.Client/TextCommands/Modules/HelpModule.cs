using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using TobysBot.Discord.Client.TextCommands.Extensions;

namespace TobysBot.Discord.Client.TextCommands.Modules;

[Group("help")]
public class HelpModule : ModuleBase<SocketCommandContext>
{
    private readonly CommandService _commandService;

    public HelpModule(CommandService commandService)
    {
        _commandService = commandService;
    }

    [Command]
    public async Task HelpAsync()
    {
        var embed = new EmbedBuilder()
            .WithAuthor("Toby's Bot Commands")
            .WithContext(EmbedContext.Information);
        
        foreach (var module in _commandService.Modules)
        {
            if (module.IsSubmodule)
            {
                continue;
            }

            if (module.Attributes.OfType<HelpCategoryAttribute>().FirstOrDefault() is not { } attribute)
            {
                continue;
            }

            embed.AddField(field =>
            {
                field.Name = module.Name;
                field.Value = $"`\\help {attribute.Name}`";
                field.IsInline = true;
            });
        }

        await Context.Message.ReplyAsync(embed: embed.Build());
    }

    [Command]
    public async Task HelpAsync(string module)
    {
        var moduleInfo = _commandService.Modules.FirstOrDefault(x => x.Attributes.OfType<HelpCategoryAttribute>().FirstOrDefault()?.Name == module);

        if (moduleInfo is null)
        {
            await Context.Message.ReplyAsync(embed: new EmbedBuilder()
                .WithContext(EmbedContext.Error)
                .WithDescription($"Could not find `{module}` module.")
                .Build());
            return;
        }

        var embed = new EmbedBuilder()
            .WithAuthor($"{moduleInfo.Name} Commands")
            .WithContext(EmbedContext.Information);

        foreach (var command in moduleInfo.Commands)
        {
            var sb = new StringBuilder();

            sb.Append($"\\{command.Aliases[0]}");

            foreach (var param in command.Parameters)
            {
                sb.Append(" [");
                    
                sb.Append(param.Name);

                if (param.IsOptional)
                {
                    sb.Append('?');
                }

                sb.Append(']');
            }
            
            embed.AddField(field =>
            {
                field.Name = sb.ToString();
                field.Value = command.Summary;
            });
        }

        try
        {
            await Context.Message.ReplyAsync(embed: embed.Build());
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}