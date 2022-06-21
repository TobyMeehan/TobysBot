using Discord.Commands;

namespace TobysBot.Commands.Builders;

public class ModuleBuilder : IModule
{
    IReadOnlyCollection<ICommand> IModule.Commands => Commands.Values;
    public Dictionary<string, CommandBuilder> Commands { get; } = new();

    public ModuleBuilder WithName(string name)
    {
        Name = name;

        return this;
    }
    public string? Name { get; set; }

    public ModuleBuilder AddCommand(CommandBuilder builder)
    {
        if (builder.Name is null)
        {
            throw new NullReferenceException("Command name was null.");
        }

        if (!Commands.TryGetValue(builder.Name, out var command))
        {
            command = new CommandBuilder()
                .WithName(builder.Name)
                .WithDescription(builder.Description);

            Commands[builder.Name] = command;
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

    public static ModuleBuilder Parse(ModuleInfo module)
    {
        var builder = new ModuleBuilder()
            .WithName(module.Name);

        foreach (var command in module.Commands)
        {
            builder.AddCommand(CommandBuilder.Parse(command));
        }

        return builder;
    }
}