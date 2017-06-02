using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using Discord.Net.WebSockets;
using Newtonsoft.Json;
using Discord.Logging;
namespace EndzoneDiscordBot
{
    class Program
    {
        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            DiscordBot discordBot = new DiscordBot();

            await discordBot.RunAsync();
            // Infinite wait
            await Task.Delay(-1);
        }
    }
}
