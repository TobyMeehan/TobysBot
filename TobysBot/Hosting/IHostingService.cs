namespace TobysBot.Hosting;

public interface IHostingService
{
    Uri Uri { get; }
    string ServerName => Uri.Authority;
}