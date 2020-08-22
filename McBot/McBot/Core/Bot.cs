using McBot.Contracts;
using McBot.HttpApi.Payloads;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace McBot.Core
{
    public class Bot
    {
        private readonly IDiscordHttpApi _discordApi;
        private readonly IDiscordWebSocketApi _discordWebSocketApi;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptions<AppSettings> _options;
        private readonly Process Server;

        public Bot(IDiscordHttpApi discordApi, IDiscordWebSocketApi discordWebSocketApi, IHttpClientFactory httpClientFactory, IOptions<AppSettings> options)
        {
            _discordApi = discordApi;
            _discordWebSocketApi = discordWebSocketApi;
            _httpClientFactory = httpClientFactory;
            _options = options;
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

                _ = StartMcServer();
            }
        }

        public async Task StartMcServer()
        {
            try
            {
                var path = Path.Combine(_options.Value.McServerPath, "run.bat");
                var processInfo = new ProcessStartInfo(path);
                processInfo.CreateNoWindow = false;
                processInfo.UseShellExecute = false;

                //processInfo.RedirectStandardOutput = true;
                //processInfo.RedirectStandardError = true;

                var Server = Process.Start(processInfo);
                if (Server.HasExited == false)
                {
                    Message successMessage = new Message("Hello everybody, server is up, server" + " IpAddress: " + await GetMyIp(), false, null);
                    var successResponse = await _discordApi.CreateMessage(successMessage);

                    Console.WriteLine(await successResponse.Content.ReadAsStringAsync());
                }
                Server.Exited += new EventHandler(ServerExited);

                Server.WaitForExit();
                Server.Close();

                Message downMessage = new Message("Hello everybody, server is currently down", false, null);
                var downResponse = await _discordApi.CreateMessage(downMessage);

                Console.WriteLine(await downResponse.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void ServerExited(object sender, System.EventArgs e)
        {
            var messageText = "Server exited and is currently dowm \n" +
                 $"Exit time    : {Server.ExitTime}\n" +
                 $"Exit code    : {Server.ExitCode}\n" +
                 $"Elapsed time : {Math.Round((Server.ExitTime - Server.StartTime).TotalMilliseconds)}";

            Message downMessage = new Message(messageText, false, null);
            _discordApi.CreateMessage(downMessage);
        }
    }
}