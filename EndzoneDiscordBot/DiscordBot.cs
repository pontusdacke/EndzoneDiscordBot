using Discord;
using Discord.WebSocket;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndzoneDiscordBot
{
    class DiscordBot
    {
        DiscordSocketClient client;
        List<IModule> modules;

        public DiscordBot()
        {
            client = new DiscordSocketClient();
            client.MessageUpdated += MessageUpdated;

            modules = new List<IModule>();
            modules.Add(new EndzoneNewsModule());
        }

        public async Task RunAsync()
        {
            await client.LoginAsync(TokenType.Bot, Constants.DiscordBotToken);
            await client.StartAsync();

            client.Ready += () =>
            {
                Console.WriteLine("Bot is connected!");

                foreach (var module in modules)
                {
                    module.RunAsync(client);
                }

                return Task.CompletedTask;
            };
        }

        private async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
        {
            var message = await before.GetOrDownloadAsync();
            Console.WriteLine($"{message} -> {after}");
        }
    }
}
