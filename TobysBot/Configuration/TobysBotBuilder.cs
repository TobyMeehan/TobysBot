using System.Reflection;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using TobysBot.Commands;

namespace TobysBot.Configuration;

public class TobysBotBuilder
{
    public IServiceCollection Services { get; }
    public ModuleCollection Modules { get; } = new();

    public TobysBotBuilder(IServiceCollection services)
    {
        Services = services;
        services.AddSingleton(Modules);
    }

    public TobysBotBuilder AddModule<T>() where T : IModuleBase
    {
        Modules.AddModule<T>();

        return this;
    }

    public TobysBotBuilder AddModulesFromAssembly(Assembly assembly)
    {
        Modules.AddModulesFromAssembly(assembly);

        return this;
    }
}