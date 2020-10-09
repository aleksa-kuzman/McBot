using MC_Server_Starter.Gateway.Payloads;
using McBot.Contracts;
using McBot.HttpApi.Payloads;
using Microsoft.Extensions.Options;
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
        private readonly IOptions<AppSettings> _options;

        public DiscordHttpApi(IHttpClientFactory httpClientFactpry, IOptions<AppSettings> options)
        {
            _httpClientFactpry = httpClientFactpry;
            _options = options;
        }

        public async Task<GatewayResource> GetWebSocketBotGateway()
        {
            var httpClient = _httpClientFactpry.CreateClient("DiscordHttpApi");

            HttpResponseMessage message = await httpClient.GetAsync("gateway/bot");
            var somethingElse = JsonConvert.DeserializeObject<GatewayResource>(await message.Content.ReadAsStringAsync());

            Console.WriteLine(somethingElse);

            return somethingElse;
        }

        public async Task<HttpResponseMessage> CreateMessage(Message message, string channelId)
        {
            var httpClient = _httpClientFactpry.CreateClient("DiscordHttpApi");

            var jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            jsonSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            var jsonObject = JsonConvert.SerializeObject(message, Formatting.Indented, jsonSettings);
            Console.WriteLine(jsonObject);
            HttpContent content = new StringContent(jsonObject, Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = await httpClient.PostAsync($"channels/{channelId}/messages", content);

            return responseMessage;
        }
    }
}