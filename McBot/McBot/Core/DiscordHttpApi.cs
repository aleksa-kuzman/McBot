using MC_Server_Starter.Gateway.Payloads;
using McBot.Contracts;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace McBot.Core
{
    public class DiscordHttpApi : IDiscordHttpApi
    {
        private readonly IHttpClientFactory _httpClientFactpry;

        public DiscordHttpApi(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactpry = httpClientFactory;
        }

        public async Task<GatewayResource> GetWebSocketBotGateway()
        {
            var httpClient = _httpClientFactpry.CreateClient("API");
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bot", "NzQxMzUyOTczMTAzMjY3OTgy.Xy2Uwg.OSMLFuKsMX399XwkW6AiA4KXURw");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "myTestAp | this is a");

            HttpResponseMessage message = await httpClient.GetAsync("https://discord.com/api/gateway/bot");
            var somethingElse = JsonConvert.DeserializeObject<GatewayResource>(await message.Content.ReadAsStringAsync());

            Console.WriteLine(somethingElse);

            return somethingElse;
        }
    }
}