using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using TobysBot.Hosting;

namespace TobysBot.Web.Hosting;

public class WebHostingService : IHostingService
{
    public WebHostingService(IServer server)
    {
        var address = server.Features.Get<IServerAddressesFeature>();

        Uri = new Uri(address?.Addresses.FirstOrDefault(addr => addr.Any(char.IsLetter)) ?? "https://bot.tobymeehan.com");
    }

    public Uri Uri { get; }
}