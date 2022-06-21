using TobysBot.Extensions;

namespace TobysBot.Commands.Builders;

public class PluginBuilder : IPlugin
{
    IReadOnlyCollection<IModule> IPlugin.Modules => Modules;
    public List<ModuleBuilder> Modules { get; } = new();
    ICommandDictionary<ICommand> IPlugin.Commands => Commands;
    public CommandDictionary Commands => Modules.SelectMany(x => x.Commands).ToCommandDictionary();

    public PluginBuilder WithId(string id)
    {
        Id = id;

        return this;
    }
    public string? Id { get; set; }
    
    public PluginBuilder WithName(string name)
    {
        Name = name;

        return this;
    }
    public string? Name { get; set; }

    public PluginBuilder WithDescription(string description)
    {
        Description = description;

        return this;
    }
    public string? Description { get; set; }

    public PluginBuilder AddModule(ModuleBuilder module)
    {
        Modules.Add(module);

        return this;
    }
}