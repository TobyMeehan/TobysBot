using Discord;
using Discord.Commands;
using TobysBot.Extensions;

namespace TobysBot.Commands.Builders;

public class CommandOptionBuilder : ICommandOption
{
    public CommandOptionBuilder WithName(string name)
    {
        Name = name;

        return this;
    }
    public string? Name { get; set; }

    public CommandOptionBuilder WithDescription(string description)
    {
        Description = description;

        return this;
    }
    public string? Description { get; set; }

    public CommandOptionBuilder WithRequired(bool required)
    {
        Required = required;

        return this;
    }
    public bool Required { get; set; }

    public CommandOptionBuilder WithType(Type type)
    {
        Type = type;

        return this;
    }
    public Type? Type { get; set; }

    public SlashCommandOptionBuilder Build()
    {
        switch (this)
        {
            case {Name: null}:
                throw new Exception("Slash command name cannot be null.");
            case {Description: null}:
                throw new Exception($"Slash command {Name} description cannot be null.");
            case {Type: null}:
                throw new Exception($"Slash command {Name} type cannot be null.");
        }
        
        return new SlashCommandOptionBuilder()
            .WithName(Name)
            .WithDescription(Description)
            .WithRequired(Required)
            .WithType(GetSlashCommandType(Type));
    }

    private static ApplicationCommandOptionType GetSlashCommandType(Type type)
    {
        if (type == typeof(bool))
        {
            return ApplicationCommandOptionType.Boolean;
        }

        if (type == typeof(string))
        {
            return ApplicationCommandOptionType.String;
        }

        if (type == typeof(int) || type == typeof(long))
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

        throw new ArgumentOutOfRangeException(nameof(type), "Could not parse slash command type.");
    }

    public static CommandOptionBuilder Parse(ParameterInfo parameterInfo)
    {
        var builder = new CommandOptionBuilder()
            .WithName(parameterInfo.Name)
            .WithDescription(parameterInfo.Summary)
            .WithRequired(!parameterInfo.IsOptional)
            .WithType(parameterInfo.Type);

        return builder;
    }
}