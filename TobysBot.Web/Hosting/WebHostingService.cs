using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using TobysBot.Hosting;

namespace TobysBot.Web.Hosting;

public class WebHostingService : IHostingService
{
    public WebHostingService(IServer server)
    {
        var address = server.Features.Get<IServerAddressesFeature>();

        Hostname = address?.Addresses.FirstOrDefault() ?? "";
    }
    
    public string Hostname { get; }
}