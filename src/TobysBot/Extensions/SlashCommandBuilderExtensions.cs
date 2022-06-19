using Discord;
using Discord.Commands;
using Discord.WebSocket;
using TobysBot.Commands;

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

    public static SlashCommandBuilder AddOption(this SlashCommandBuilder builder, ParameterInfo parameter)
    {
        var optionBuilder = new SlashCommandOptionBuilder()
            .WithName(parameter.Name)
            .WithDescription(parameter.Summary)
            .WithRequired(!parameter.IsOptional)
            .WithType(parameter.Type.ToSlashCommandType());

        foreach (var choice in parameter.Attributes.OfType<ChoiceAttribute>())
        {
            switch (choice.Value)
            {
                case string s:
                    optionBuilder.AddChoice(choice.Name, s);
                    break;
                case int i:
                    optionBuilder.AddChoice(choice.Name, i);
                    break;
                case double d:
                    optionBuilder.AddChoice(choice.Name, d);
                    break;
                case float f:
                    optionBuilder.AddChoice(choice.Name, f);
                    break;
                case long l:
                    optionBuilder.AddChoice(choice.Name, l);
                    break;
            }
        }

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

    public static SlashCommandOptionBuilder AddOption(this SlashCommandOptionBuilder builder, ParameterInfo parameter)
    {
        var optionBuilder = new SlashCommandOptionBuilder()
            .WithName(parameter.Name)
            .WithDescription(parameter.Summary)
            .WithRequired(!parameter.IsOptional)
            .WithType(parameter.Type.ToSlashCommandType());

        foreach (var choice in parameter.Attributes.OfType<ChoiceAttribute>())
        {
            switch (choice.Value)
            {
                case string s:
                    optionBuilder.AddChoice(choice.Name, s);
                    break;
                case int i:
                    optionBuilder.AddChoice(choice.Name, i);
                    break;
                case double d:
                    optionBuilder.AddChoice(choice.Name, d);
                    break;
                case float f:
                    optionBuilder.AddChoice(choice.Name, f);
                    break;
                case long l:
                    optionBuilder.AddChoice(choice.Name, l);
                    break;
            }
        }
        
        return builder.AddOption(optionBuilder);
    }

    public static SlashCommandOptionBuilder AddOptions(this SlashCommandOptionBuilder builder,
        IEnumerable<ParameterInfo> parameters)
    {
        foreach (var parameter in parameters)
        {
            builder.AddOption(parameter);
        }

        return builder;
    }

    public static List<SlashCommandBuilder> AddModules(this List<SlashCommandBuilder> collection,
        IEnumerable<ModuleInfo> modules)
    {
        foreach (var module in modules)
        {
            collection.AddModule(module);
        }

        return collection;
    }

    public static List<SlashCommandBuilder> AddModule(this List<SlashCommandBuilder> collection,
        ModuleInfo module)
    {
        if (module.Attributes.OfType<PluginAttribute>().Any() && !module.IsSubmodule)
        {
            return collection;
        }

        if (module.Group is null)
        {
            collection.AddCommands(module);
            return collection;
        }

        if (module.Submodules.Any())
        {
            collection.AddModuleGroup(module);
        }

        if (module.Commands.Any())
        {
            collection.AddGroup(module);
        }

        return collection;
    }

    public static List<SlashCommandBuilder> AddCommands(this List<SlashCommandBuilder> collection,
        ModuleInfo module)
    {
        var groups =
            from command in module.Commands
            group command by command.SubCommandGroup()
            into newGroup1
            from newGroup2 in (
                from command in newGroup1
                group command by command.SubCommandParent()
            )
            group newGroup2 by newGroup1.Key;


        foreach (var command in groups)
        {
            if (command.Key is not null)
            {
                collection.AddSubCommandGroup(command);
                continue;
            }

            foreach (var group in command)
            {
                if (group.Key is not null)
                {
                    collection.AddSubCommands(group);
                    continue;
                }

                collection.AddCommands(group);
            }
        }

        return collection;
    }

    public static List<SlashCommandBuilder> AddSubCommandGroup(this List<SlashCommandBuilder> collection,
        IGrouping<string, IGrouping<string, CommandInfo>> command)
    {
        var commandBuilder = new SlashCommandBuilder()
            .WithName(command.Key)
            .WithDescription("Foo");

        foreach (var group in command)
        {
            var optionBuilder = new SlashCommandOptionBuilder()
                .WithName(group.Key)
                .WithDescription("Bar")
                .WithType(ApplicationCommandOptionType.SubCommandGroup);

            foreach (var subcommand in group)
            {
                optionBuilder.AddOption(new SlashCommandOptionBuilder()
                    .WithName(subcommand.Aliases[0][(command.Key.Length + group.Key.Length + 2)..])
                    .WithDescription(subcommand.Summary)
                    .AddOptions(subcommand.Parameters)
                    .WithType(ApplicationCommandOptionType.SubCommand));
            }

            commandBuilder.AddOption(optionBuilder);
        }

        collection.Add(commandBuilder);

        return collection;
    }

    public static List<SlashCommandBuilder> AddSubCommands(this List<SlashCommandBuilder> collection,
        IGrouping<string, CommandInfo> command)
    {
        var commandBuilder = new SlashCommandBuilder()
            .WithName(command.Key)
            .WithDescription("Foo");

        foreach (var subcommand in command)
        {
            commandBuilder.AddOption(new SlashCommandOptionBuilder()
                .WithName(subcommand.Aliases[0][(command.Key.Length + 1)..])
                .WithDescription(subcommand.Summary)
                .AddOptions(subcommand.Parameters)
                .WithType(ApplicationCommandOptionType.SubCommand));
        }

        collection.Add(commandBuilder);

        return collection;
    }

    public static List<SlashCommandBuilder> AddCommands(this List<SlashCommandBuilder> collection,
        IEnumerable<CommandInfo> commands)
    {
        foreach (var command in commands)
        {
            collection.Add(new SlashCommandBuilder()
                .WithName(command.Aliases[0])
                .WithDescription(command.Summary)
                .AddOptions(command.Parameters));
        }

        return collection;
    }

    public static List<SlashCommandBuilder> AddModuleGroup(this List<SlashCommandBuilder> collection,
        ModuleInfo module)
    {
        var command = new SlashCommandBuilder()
            .WithName(module.Group)
            .WithDescription(module.Summary);

        foreach (var group in module.Submodules)
        {
            var commandGroup = new SlashCommandOptionBuilder()
                .WithName(group.Summary)
                .WithDescription(group.Summary)
                .WithType(ApplicationCommandOptionType.SubCommandGroup);

            foreach (var subcommand in group.Commands)
            {
                commandGroup.AddOption(new SlashCommandOptionBuilder()
                    .WithName(subcommand.Aliases[0])
                    .WithDescription(subcommand.Summary)
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOptions(subcommand.Parameters));
            }

            command.AddOption(commandGroup);
        }

        collection.Add(command);

        return collection;
    }

    public static List<SlashCommandBuilder> AddGroup(this List<SlashCommandBuilder> collection,
        ModuleInfo module)
    {
        var command = new SlashCommandBuilder()
            .WithName(module.Group)
            .WithDescription(module.Summary);

        foreach (var subcommand in module.Commands)
        {
            command.AddOption(new SlashCommandOptionBuilder()
                .WithName(subcommand.Aliases[0])
                .WithDescription(subcommand.Summary)
                .WithType(ApplicationCommandOptionType.SubCommand)
                .AddOptions(subcommand.Parameters));
        }

        collection.Add(command);

        return collection;
    }

    public static async Task InstallSlashCommandsAsync(this DiscordSocketClient client, IEnumerable<ModuleInfo> modules)
    {
        var commands = new List<SlashCommandBuilder>()
            .AddModules(modules);

        foreach (var guild in client.Guilds)
        {
            await guild.BulkOverwriteApplicationCommandAsync(commands
                .Select(x => x.Build())
                .Cast<ApplicationCommandProperties>()
                .ToArray());
        }
    }
}