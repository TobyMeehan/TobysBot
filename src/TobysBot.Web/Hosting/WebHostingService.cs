using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using TobysBot.Hosting;

namespace TobysBot.Web.Hosting;

public class WebHostingService : IHostingService
{
    public WebHostingService(IServer server)
    {
        var address = server.Features.Get<IServerAddressesFeature>();
        var defaultUri = new Uri("https://bot.tobymeehan.com");

        if (address is null)
        {
            Uri = defaultUri;
            return;
        }

        var uris = address.Addresses.Select(x => new Uri(x));

        Uri = uris.FirstOrDefault(x => x.Host.All(c => char.IsLetter(c) || c is '.')) ?? defaultUri;
    }

    public Uri Uri { get; }
}