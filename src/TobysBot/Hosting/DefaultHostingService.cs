namespace TobysBot.Hosting;

public class DefaultHostingService : IHostingService
{
    public Uri Uri => new("https://bot.tobymeehan.com");
}