using Discord;
using Discord.Commands;

namespace TobysBot.Extensions;

public static class SlashCommandBuilderExtensions
{
    public static ApplicationCommandOptionType ToSlashCommandType(this Type type)
    {
        if (type.IsEnum)
        {
            return Enum.GetUnderlyingType(type).ToSlashCommandType();
        }
        
        if (type == typeof(bool))
        {
            return ApplicationCommandOptionType.Boolean;
        }
        
        if (type == typeof(string))
        {
            return ApplicationCommandOptionType.String;
        }

        if (type == typeof(int))
        {
            return ApplicationCommandOptionType.Integer;
        }

        if (type == typeof(float) || type == typeof(double) || type == typeof(decimal))
        {
            return ApplicationCommandOptionType.Number;
        }

        if (type.IsAssignableTo<IChannel>())
        {
            return ApplicationCommandOptionType.Channel;
        }
        
        if (type.IsAssignableTo<IUser>())
        {
            return ApplicationCommandOptionType.User;
        }

        if (type.IsAssignableTo<IRole>())
        {
            return ApplicationCommandOptionType.Role;
        }

        if (type.IsAssignableTo<IMentionable>())
        {
            return ApplicationCommandOptionType.Mentionable;
        }

        throw new ArgumentOutOfRangeException(nameof(type),"Could not parse slash command type.");
    }
    
    public static SlashCommandBuilder AddOption(this SlashCommandBuilder builder, ParameterInfo parameter)
    {
        var optionBuilder = new SlashCommandOptionBuilder()
            .WithName(parameter.Name)
            .WithDescription(parameter.Summary)
            .WithRequired(!parameter.IsOptional)
            .AddChoices(parameter.Type)
            .WithType(parameter.Type.ToSlashCommandType());

        return builder.AddOption(optionBuilder);
    }

    public static SlashCommandBuilder AddOptions(this SlashCommandBuilder builder,
        IEnumerable<ParameterInfo> parameters)
    {
        foreach (var parameter in parameters)
        {
            builder.AddOption(parameter);
        }

        return builder;
    }

    public static SlashCommandOptionBuilder AddChoices(this SlashCommandOptionBuilder builder, Type enumType)
    {
        if (!enumType.IsEnum)
        {
            return builder;
        }

        foreach (var choice in Enum.GetValues(enumType))
        {
            var name = Enum.GetName(enumType, choice)?.ToLower();
            
            switch (choice)
            {
                case byte b:
                    builder.AddChoice(name, b);
                    break;
                case sbyte sb:
                    builder.AddChoice(name, sb);
                    break;
                case short s:
                    builder.AddChoice(name, s);
                    break;
                case ushort us:
                    builder.AddChoice(name, us);
                    break;
                case int i:
                    builder.AddChoice(name, i);
                    break;
                case uint ui:
                    builder.AddChoice(name, ui);
                    break;
                case long l:
                    builder.AddChoice(name, l);
                    break;
                case ulong ul:
                    builder.AddChoice(name, ul);
                    break;
            }
        }

        return builder;
    }

    public static async Task AddSlashCommandsAsync(this IGuild guild, IEnumerable<SlashCommandProperties> cmds)
    {
        var commands = cmds.ToList();
        var commandNames = commands.Select(x => x.Name.Value);
        
        var existing = await guild.GetApplicationCommandsAsync();

        foreach (var command in existing.Where(x => !commandNames.Contains(x.Name)))
        {
            await command.DeleteAsync();
        }
        
        foreach (var command in commands)
        {
            await guild.CreateApplicationCommandAsync(command);
        }
    }
}