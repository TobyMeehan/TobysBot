namespace TobysBot.Hosting;

public class DefaultHostingService : IHostingService
{
    public Uri Uri => new Uri("https://bot.tobymeehan.com");
}