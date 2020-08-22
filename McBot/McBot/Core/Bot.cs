using McBot.Contracts;
using McBot.HttpApi.Payloads;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace McBot.Core
{
    public class Bot
    {
        private readonly IDiscordHttpApi _discordApi;
        private readonly IDiscordWebSocketApi _discordWebSocketApi;
        private readonly IHttpClientFactory _httpClientFactory;

        public Bot(IDiscordHttpApi discordApi, IDiscordWebSocketApi discordWebSocketApi, IHttpClientFactory httpClientFactory)
        {
            _discordApi = discordApi;
            _discordWebSocketApi = discordWebSocketApi;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GetMyIp()
        {
            HttpClient client = _httpClientFactory.CreateClient("WhatisMyIpApi");

            HttpResponseMessage message = await client.GetAsync("");
            string ipAddress = await message.Content.ReadAsStringAsync();

            return ipAddress;
        }

        //TODO: Need to refactor connection logic away from the bot, still is a little too messy
        public async Task RunAsync()
        {
            var gateway = await _discordApi.GetWebSocketBotGateway();
            var conected = await _discordWebSocketApi.ConnectToSocketApi(gateway.Url);

            if (conected != null)
            {
                _ = _discordWebSocketApi.SendHearthBeat(conected.GatewayHello.heartbeat_interval);

                var identification = await _discordWebSocketApi.IdentifyToSocket(gateway.Url);

                if (identification.op == OpCode.InvalidSession)
                {
                    throw new System.Exception("API RETURNED OPCODE 9");
                }
                else if (identification.op == 0)
                {
                    Console.WriteLine(identification.Ready.session_id);
                }

                Message testMessage = new Message("Hello everybody, current server" + " IpAddress: " + await GetMyIp(), false, null);
                var createResponse = await _discordApi.CreateMessage(testMessage);

                Console.WriteLine(await createResponse.Content.ReadAsStringAsync());
            }
        }
    }
}