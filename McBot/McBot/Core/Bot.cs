using McBot.Contracts;
using McBot.Gateway.Payloads;
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
        private bool forcedExit;
        private Process Server;

        public Bot(IDiscordHttpApi discordApi, IDiscordWebSocketApi discordWebSocketApi, IHttpClientFactory httpClientFactory, IOptions<AppSettings> options)
        {
            _discordApi = discordApi;
            _discordWebSocketApi = discordWebSocketApi;
            _httpClientFactory = httpClientFactory;
            _options = options;
            Server = new Process();
            forcedExit = true;

            _discordWebSocketApi.RespondToCreateMessage += SendNudesRespond;
            _discordWebSocketApi.RespondToCreateMessage += StartServerRespond;
            _discordWebSocketApi.RespondToCreateMessage += KillServerRespond;
            _discordWebSocketApi.RespondToCreateMessage += VoiceConnectRespond;
        }

        public async Task<string> GetMyIp()
        {
            HttpClient client = _httpClientFactory.CreateClient("WhatisMyIpApi");

            HttpResponseMessage message = await client.GetAsync("");
            string ipAddress = await message.Content.ReadAsStringAsync();

            return ipAddress;
        }

        // var response = await _discordApi.CreateMessage(new Message(message, false, null), channelId);

        private async Task VoiceConnect()
        {
            var gateway = await _discordApi.GetWebSocketBotGateway();

            _ = _discordApi.CreateMessage(new Message("Connecting to voice channel", false, null), _options.Value.ChannelId);

            var voiceStateUpdate = new VoiceStateUpdate("741382226746409011", "741382227249856545", false, false);
            await _discordWebSocketApi.ConnectToVoice(voiceStateUpdate);
        }

        private async Task VoiceConnectRespond(MessageCreated message)
        {
            if (message != null)
            {
                if (message.Content == "voiceConnect()")
                {
                    await VoiceConnect();
                }
            }
        }

        private async Task SendNudesRespond(MessageCreated message)
        {
            if (message != null)
            {
                if (message.Content == "sendNudes()")
                {
                    var response = await _discordApi.CreateMessage(new Message("Hey don't do that asshole", false, null), _options.Value.ChannelId);
                }
            }
        }

        public async Task KillServer()
        {
            forcedExit = false;
            Server.Kill();
            var response = await _discordApi.CreateMessage(new Message("You killed a server", false, null), _options.Value.ChannelId);

            /* var messageText = $"Server has shut down and is currently down \n";
             await SendMessageToChannel(messageText, _options.Value.ChannelId);*/

            Server.Dispose();
            Server = new Process();
        }

        private async Task KillServerRespond(MessageCreated message)
        {
            if (message != null)
            {
                if (message.Content == "killServer()")
                {
                    await KillServer();
                }
            }
        }

        private async Task StartServerRespond(MessageCreated message)
        {
            if (message != null)
            {
                if (message.Content == "startServer()")
                {
                    await StartMcServer();
                }
            }
        }

        //TODO: Need to refactor connection logic away from the bot, still is a little too messy
        public async Task RunAsync()
        {
            var gateway = await _discordApi.GetWebSocketBotGateway();
            var conected = await _discordWebSocketApi.ConnectToSocketApi(gateway.Url);

            if (conected != null)
            {
                _ = _discordWebSocketApi.SendHearthBeat(conected.heartbeat_interval);

                var identification = await _discordWebSocketApi.IdentifyToSocket(gateway.Url);

                await StartMcServer();

                while (true)
                {
                    var test = await _discordWebSocketApi.MessageCreatedEvent();
                }
            }
        }

        //TODO Refactor Process away from bot
        public async Task StartMcServer()
        {
            try
            {
                var path = Path.Combine(_options.Value.McServerPath, "forge-1.16.1-32.0.66.jar");
                var processInfo = new ProcessStartInfo(@"C:\Program Files\Java\jre1.8.0_261\bin\java.exe");
                processInfo.CreateNoWindow = false;
                processInfo.UseShellExecute = false;
                processInfo.Arguments = "-Xmx2048M -Xms2048M -jar " + path;
                processInfo.WorkingDirectory = _options.Value.McServerPath;

                Server.StartInfo = processInfo;
                Server.EnableRaisingEvents = true;
                Server.Exited += new EventHandler(ServerExited);
                Server.Start();

                if (Server.HasExited == false)
                {
                    var response = await _discordApi.CreateMessage(new Message("Hello everybody, server is up, server" + " IpAddress: " + await GetMyIp(), false, null), _options.Value.ChannelId);
                    var created = await _discordWebSocketApi.MessageCreatedEvent();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void ServerExited(object sender, System.EventArgs e)
        {
            try
            {
                if (forcedExit)
                {
                    _ = KillServer();
                }
                forcedExit = true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}