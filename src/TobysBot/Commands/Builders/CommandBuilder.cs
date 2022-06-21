using Discord;
using Discord.Commands;
using TobysBot.Extensions;

namespace TobysBot.Commands.Builders;

public class CommandBuilder : ICommand
{
    
    public CommandDictionary SubCommands { get; } = new();
    public List<CommandOptionBuilder> Options { get; } = new();

    ICommandDictionary<ICommand> ICommand.SubCommands => SubCommands;
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

    public CommandBuilder WithUsages(IEnumerable<CommandUsage> usages)
    {
        Usages = usages.ToList();

        return this;
    }
    public IReadOnlyCollection<CommandUsage> Usages { get; set; }

    public CommandBuilder AddSubCommand(CommandBuilder builder)
    {
        if (builder.Name is null)
        {
            throw new NullReferenceException("Subcommand name was null.");
        }

        var command = SubCommands[builder.Name]
            .WithDescription(builder.Description);

        foreach (var subCommand in builder.SubCommands)
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

    public CommandBuilder WithExecute(Func<ICommandContext, IEnumerable<object>, IEnumerable<object>, IServiceProvider, Task<IResult>> execute)
    {
        Execute = execute;

        return this;
    }
    public Func<ICommandContext, IEnumerable<object>, IEnumerable<object>, IServiceProvider, Task<IResult>>? Execute { get; set; }
    
    public CommandBuilder WithCheckPreconditions(Func<ICommandContext, IServiceProvider, Task<IResult>> checkPreconditions)
    {
        CheckPreconditions = checkPreconditions;

        return this;
    }
    public Func<ICommandContext, IServiceProvider, Task<IResult>>? CheckPreconditions { get; set; }
    
    public ExecutableCommandBuilder Executable(IServiceProvider services)
    {
        return new ExecutableCommandBuilder(this, services);
    }

    public SlashCommandProperties Build()
    {
        var builder = new SlashCommandBuilder()
            .WithName(Name)
            .WithDescription(Description);

        foreach (var subCommand in SubCommands)
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

        foreach (var subCommand in SubCommands)
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

        return builder
            .WithDescription(commandInfo.Summary)
            .WithUsages(commandInfo.Usage())
            .WithCheckPreconditions(async (context, services) => await commandInfo.CheckPreconditionsAsync(context, services))
            .WithExecute(commandInfo.ExecuteAsync);
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