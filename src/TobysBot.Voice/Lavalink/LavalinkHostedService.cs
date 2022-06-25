using Microsoft.Extensions.Hosting;
using TobysBot.Events;

namespace TobysBot.Voice.Lavalink;

public class LavalinkHostedService : IHostedService, IEventHandler<DiscordClientReadyEventArgs>
{
    private readonly ILavalinkNode _lavaNode;

    public LavalinkHostedService(ILavalinkNode lavaNode)
    {
        _lavaNode = lavaNode;
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
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _lavaNode.DisconnectAsync();
    }
}