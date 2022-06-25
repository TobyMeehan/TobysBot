using Microsoft.AspNetCore.Mvc;
using TobysBot.Commands;
using TobysBot.Web.Models;

namespace TobysBot.Web.Controllers;

[ApiController]
[Route("/data/commands")]
public class CommandController : ControllerBase
{
    private readonly ICommandService _commands;

    public CommandController(ICommandService commands)
    {
        _commands = commands;
    }

    [HttpGet("commands")]
    public IEnumerable<Command> GetCommands()
    {
        return _commands.Plugins.SelectMany(plugin => 
            plugin.Commands.SelectMany(command => 
                command.Usages.Select(usage =>
                    new Command
                    {
                        Name = command.Name,
                        PluginId = plugin.Id,
                        Parameters = usage.Parameters,
                        Description = command.Description
                    })));
    }

    [HttpGet("plugins")]
    public IEnumerable<Plugin> GetPlugins()
    {
        return _commands.Plugins.Select(x => new Plugin
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description
        });
    }
}