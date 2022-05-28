using Microsoft.Extensions.DependencyInjection;

namespace TobysBot.Voice.Configuration;

public static class ServiceCollectionExtensions
{
    public static VoiceModuleBuilder AddVoiceModule(this IServiceCollection services)
    {
        return new VoiceModuleBuilder(services);
    }
}