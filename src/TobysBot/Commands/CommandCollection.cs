using System.Reflection;
using Discord.Commands;

namespace TobysBot.Commands;

public class CommandCollection
{
    private readonly List<Type> _modules = new();
    private readonly List<Assembly> _assemblies = new();
    private readonly List<(Type Type, TypeReader TypeReader)> _typeReaders = new();
    
    public void AddPlugin<T>() where T : PluginBase
    {
        _modules.Add(typeof(T));
    }

    public void AddPluginFromAssembly(Assembly assembly, string name, string summary)
    {
        _assemblies.Add(assembly);
    }

    public void AddGlobalModule<T>() where T : IModuleBase
    {
        _modules.Add(typeof(T));
    }

    public void AddTypeReader<TType, TReader>() where TReader : TypeReader, new()
    {
        _typeReaders.Add((typeof(TType), new TReader()));
    }

    public IEnumerable<Type> GetModules() => _modules;
    public IEnumerable<Assembly> GetAssemblies() => _assemblies;
    public IEnumerable<(Type Type, TypeReader TypeReader)> GetTypeReaders() => _typeReaders;
}