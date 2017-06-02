using Discord.WebSocket;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EndzoneDiscordBot
{
    class EndzoneNewsModule : IModule
    {
        GoogleUrlShortener googleUrlShortener;
        HtmlWeb web;
        HtmlDocument document;

        public EndzoneNewsModule()
        {
            googleUrlShortener = new GoogleUrlShortener(Constants.GoogleApiKey);
            web = new HtmlWeb();
            document = web.Load(Constants.EndzoneNewsUrl);
        }

        public async Task RunAsync(DiscordSocketClient client)
        {
            while (true)
            {
                try
                {
                    var latestNewsLink = GetLatestNewsLink();

                    if (IsNewsAlreadyPosted(latestNewsLink))
                    {
                        await Task.Delay(Constants.NewsCheckDelayMs);
                        continue;
                    }
                    else
                    {
                        var title = GetLatestNewsTitle();
                        var shortLink = await googleUrlShortener.Shorten(latestNewsLink);
                        var imageUrl = GetNewsImageUrl();

                        string imageFileName = Path.GetFileName(imageUrl);
                        using (WebClient webClient = new WebClient())
                        {
                            webClient.DownloadFile(new Uri(imageUrl), imageFileName);
                        }

                        await SendNewsAsync(client, imageFileName, $"{title}: {shortLink}");
                        SaveLinkToFile(latestNewsLink);
                        Console.WriteLine("Nyhet visad: " + title);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Något gick fel med nyhetskollen. " + e);
                }

                await Task.Delay(Constants.NewsCheckDelayMs);
            }
        }

        private void SaveLinkToFile(string link)
        {
            File.AppendAllText(Constants.NewsFilePath, link + Environment.NewLine);
        }

        private bool IsNewsAlreadyPosted(string link)
        {
            if (!File.Exists(Constants.NewsFilePath))
            {
                return false;
            }

            var lines = File.ReadAllLines(Constants.NewsFilePath);
            return lines.Any(x => x == link);
        }

        private async Task SendNewsAsync(DiscordSocketClient client, string imageFileName, string message)
        {
            var channel = client.GetChannel(Constants.EndzoneDiscordGeneralChannelId) as SocketTextChannel;
            await channel.SendFileAsync(imageFileName, message);
        }

        private string GetNewsImageUrl()
        {
            return GetArticle()
                .Descendants("img")
                .FirstOrDefault()
                ?.Attributes["src"]
                .Value;
        }

        private HtmlNode GetArticle()
        {
            return document.DocumentNode.Descendants("li")
               .Where(x => x.Attributes.Contains("class")
                        && x.Attributes["class"].Value.Contains("article-1"))
               .SingleOrDefault();
        }

        private string GetLatestNewsTitle()
        {
            return GetArticle().Descendants("h3").SingleOrDefault()?.InnerText;
        }

        private string GetLatestNewsLink()
        {
            return Constants.EndzoneBaseUrl +
                GetArticle()
                .Descendants("a")
                .Where(x => x.Attributes.Contains("class")
                    && x.Attributes["class"].Value.Contains("read-more"))
                .SingleOrDefault()
                ?.Attributes["href"]
                .Value;
        }
    }
}
