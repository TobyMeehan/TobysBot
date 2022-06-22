using Discord;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TobysBot.Configuration;

namespace TobysBot.Commands;

public class EmbedService
{
    private readonly IServiceProvider _services;
    private readonly TobysBotOptions _options;

    public TobysBotEmbedOptions? EmbedOptions => _options.Embeds;
    
    public EmbedService(IOptions<TobysBotOptions> options, IServiceProvider services)
    {
        _services = services;
        _options = options.Value;
    }

    public EmbedBuilder Builder()
    {
        return new EmbedServiceBuilder(this);
    }

    public TOptions Options<TOptions>() where TOptions : class
    {
        return _services.GetRequiredService<IOptions<TOptions>>().Value;
    }
}

public enum EmbedContext
{
    Action,
    Information,
    Error
}

public class EmbedServiceBuilder : EmbedBuilder
{
    public EmbedService Service { get; }

    public EmbedServiceBuilder(EmbedService service)
    {
        Service = service;
    }
}