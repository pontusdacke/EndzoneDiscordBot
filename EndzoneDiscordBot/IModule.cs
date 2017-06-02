using Discord.WebSocket;
using System.Threading.Tasks;

namespace EndzoneDiscordBot
{
    interface IModule
    {
        Task RunAsync(DiscordSocketClient client);
    }
}
