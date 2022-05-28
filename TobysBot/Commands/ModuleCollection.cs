using System.Reflection;
using Discord.Commands;

namespace TobysBot.Commands;

public class ModuleCollection
{
    private List<Type> _modules = new();
    private List<Assembly> _assemblies = new();

    public void AddModule<T>() where T : IModuleBase
    {
        _modules.Add(typeof(T));
    }

    public void AddModulesFromAssembly(Assembly assembly)
    {
        _assemblies.Add(assembly);
    }

    public IEnumerable<Type> GetModules() => _modules;
    public IEnumerable<Assembly> GetAssemblies() => _assemblies;
}