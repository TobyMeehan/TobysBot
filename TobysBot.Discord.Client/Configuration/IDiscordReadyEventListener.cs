using System.Threading.Tasks;

namespace TobysBot.Discord.Client.Configuration;

public interface IDiscordReadyEventListener
{
    Task OnDiscordReady();
}