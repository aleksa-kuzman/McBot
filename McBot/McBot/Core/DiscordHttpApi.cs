using MC_Server_Starter.Gateway.Payloads;
using McBot.Contracts;
using McBot.HttpApi.Payloads;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net.Http;
using System.Text;
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

        public async Task<HttpResponseMessage> CreateMessage()
        {
            var httpClient = _httpClientFactpry.CreateClient("DiscordHttpApi");
            //httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bot", "NzQxMzUyOTczMTAzMjY3OTgy.Xy2Uwg.OSMLFuKsMX399XwkW6AiA4KXURw");
            //httpClient.DefaultRequestHeaders.Add("User-Agent", "myTestAp | this is a");

            // Embed embed = new Embed("SEND", "HALP");
            var jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            jsonSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            Message message = new Message("Hey @Milos39 whatsupp :-D, I'm Pu55y Consumer", false, null);
            var jsonObject = JsonConvert.SerializeObject(message, Formatting.Indented, jsonSettings);
            Console.WriteLine(jsonObject);
            HttpContent content = new StringContent(jsonObject, Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = await httpClient.PostAsync("https://discord.com/api/channels/741382227249856544/messages", content);

            return responseMessage;
        }
    }
}