using Discord.Commands;

namespace TobysBot.Commands;

public interface IModuleRegistration
{
    Type ModuleType { get; }
}

public class ModuleRegistration<T> : IModuleRegistration where T : IModuleBase
{
    public Type ModuleType => typeof(T);
}