using Discord.Commands;

namespace TobysBot.Commands.Builders;

public class ModuleBuilder : IModule
{
    ICommandDictionary<ICommand> IModule.Commands => Commands;
    public CommandDictionary Commands { get; } = new();

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

        var command = Commands[builder.Name]
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