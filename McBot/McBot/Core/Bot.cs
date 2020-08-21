﻿using McBot.Contracts;
using McBot.HttpApi.Payloads;
using System;
using System.Threading.Tasks;

namespace McBot.Core
{
    public class Bot
    {
        private readonly IDiscordHttpApi _discordApi;
        private readonly IDiscordWebSocketApi _discordWebSocketApi;

        public Bot(IDiscordHttpApi discordApi, IDiscordWebSocketApi discordWebSocketApi)
        {
            _discordApi = discordApi;
            _discordWebSocketApi = discordWebSocketApi;
        }

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
                Message testMessage = new Message("Hello everybody", false, null);
                var createResponse = await _discordApi.CreateMessage(testMessage);

                Console.WriteLine(await createResponse.Content.ReadAsStringAsync());
            }

            while (true)
            {
                var response = _discordWebSocketApi.SendHearthBeat(41250);
            }
        }
    }
}