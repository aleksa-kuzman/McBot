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
        private readonly HttpClient _httpClient;

        public DiscordHttpApi(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GatewayResource> GetWebSocketBotGateway()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bot", "NzQxMzUyOTczMTAzMjY3OTgy.Xy2Uwg.OSMLFuKsMX399XwkW6AiA4KXURw");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "myTestAp | this is a");

            HttpResponseMessage message = await _httpClient.GetAsync("https://discord.com/api/gateway/bot");
            var somethingElse = JsonConvert.DeserializeObject<GatewayResource>(await message.Content.ReadAsStringAsync());

            Console.WriteLine(somethingElse);

            return somethingElse;
        }
    }
}