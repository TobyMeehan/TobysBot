using Microsoft.Extensions.Hosting;
using TobysBot.Events;
using TobysBot.Voice.Events;
using Victoria;
using Victoria.EventArgs;

namespace TobysBot.Voice.Lavalink;

public class LavalinkHostedService : IHostedService, IEventHandler<DiscordClientReadyEventArgs>
{
    private readonly ILavalinkNode _lavaNode;
    private readonly IEventService _events;

    public LavalinkHostedService(ILavalinkNode lavaNode, IEventService events)
    {
        _lavaNode = lavaNode;
        _events = events;
    }
    
    async Task IEventHandler<DiscordClientReadyEventArgs>.HandleAsync(DiscordClientReadyEventArgs args)
    {
        try
        {
            await _lavaNode.ConnectAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _lavaNode.DisconnectAsync();
    }
}