using Google.Apis.Services;
using Google.Apis.Urlshortener.v1;
using Google.Apis.Urlshortener.v1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndzoneDiscordBot
{
    /// <summary>
    /// Shorten URLs with Google Url Shortener API.
    /// </summary>
    class GoogleUrlShortener
    {
        UrlshortenerService service;

        /// <summary>
        /// Instatiates an object of the <see cref="GoogleUrlShortener"/> class.
        /// </summary>
        /// <param name="apiKey">Google API key.</param>
        public GoogleUrlShortener(string apiKey)
        {
            service = new UrlshortenerService(new BaseClientService.Initializer()
            {
                ApiKey = Constants.GoogleApiKey
            });
        }

        /// <summary>
        /// Shortens an url.
        /// </summary>
        /// <param name="longUrl">The long url</param>
        /// <returns>A Google short url.</returns>
        public async Task<string> Shorten(string longUrl)
        {
            var url = new Url() { LongUrl = longUrl };
            return (await service.Url.Insert(url).ExecuteAsync()).Id;
        }
    }
}
