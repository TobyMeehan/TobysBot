using Discord;
using Discord.Commands;

namespace TobysBot.Commands.Builders;

public class CommandBuilder : ICommand
{
    public Dictionary<string, CommandBuilder> SubCommands { get; } = new();
    public List<CommandOptionBuilder> Options { get; } = new();

    IReadOnlyCollection<ICommand> ICommand.SubCommands => SubCommands.Values;
    IReadOnlyCollection<ICommandOption> ICommand.Options => Options;

    public CommandBuilder WithName(string name)
    {
        Name = name;

        return this;
    }

    public string? Name { get; set; }

    public CommandBuilder WithDescription(string? description)
    {
        Description = description;

        return this;
    }

    public string? Description { get; set; }

    public CommandBuilder AddSubCommand(CommandBuilder builder)
    {
        if (builder.Name is null)
        {
            throw new NullReferenceException("Subcommand name was null.");
        }

        if (!SubCommands.TryGetValue(builder.Name, out var command))
        {
            command = new CommandBuilder()
                .WithName(builder.Name)
                .WithDescription(builder.Description);

            SubCommands[builder.Name] = command;
        }

        foreach (var subCommand in builder.SubCommands.Values)
        {
            command.AddSubCommand(subCommand);
        }

        foreach (var option in builder.Options)
        {
            command.AddOption(option);
        }

        return this;
    }

    public CommandBuilder AddOption(CommandOptionBuilder option)
    {
        Options.Add(option);

        return this;
    }

    public SlashCommandProperties Build()
    {
        var builder = new SlashCommandBuilder()
            .WithName(Name)
            .WithDescription(Description);

        foreach (var subCommand in SubCommands.Values)
        {
            builder.AddOption(subCommand.BuildSubCommand());
        }

        foreach (var option in Options)
        {
            builder.AddOption(option.Build());
        }

        return builder.Build();
    }

    public SlashCommandOptionBuilder BuildSubCommand()
    {
        var builder = new SlashCommandOptionBuilder()
            .WithName(Name)
            .WithDescription(Description);

        foreach (var subCommand in SubCommands.Values)
        {
            builder.WithType(ApplicationCommandOptionType.SubCommandGroup)
                .AddOption(subCommand.BuildSubCommand());
        }

        foreach (var option in Options)
        {
            builder.WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption(option.Build());
        }

        return builder;
    }

    public static CommandBuilder Parse(CommandInfo commandInfo)
    {
        string[] segments = commandInfo.Aliases[0].Split(" ");

        return Parse(commandInfo, segments);
    }

    private static CommandBuilder Parse(CommandInfo commandInfo, string[] segments)
    {
        var builder = new CommandBuilder()
            .WithName(segments[0])
            .WithDescription("undefined");

        if (segments.Length > 1)
        {
            return builder.AddSubCommand(Parse(commandInfo, segments[1..]));
        }

        foreach (var parameter in commandInfo.Parameters)
        {
            builder.AddOption(new CommandOptionBuilder()
                .WithName(parameter.Name)
                .WithDescription(parameter.Summary)
                .WithType(parameter.Type)
                .WithRequired(!parameter.IsOptional));
        }

        return builder.WithDescription(commandInfo.Summary);
    }

    public static CommandBuilder Parse(ModuleInfo moduleInfo)
    {
        var builder = new CommandBuilder()
            .WithName(moduleInfo.Name)
            .WithDescription(moduleInfo.Summary);

        foreach (var submodule in moduleInfo.Submodules)
        {
            builder.AddSubCommand(Parse(submodule));
        }

        foreach (var command in moduleInfo.Commands)
        {
            builder.AddSubCommand(Parse(command));
        }

        return builder;
    }
}