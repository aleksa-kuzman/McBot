﻿using McBot.Contracts;
using McBot.Gateway.Payloads;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace McBot.Core
{
    public class DiscordWebSocketApi : IDiscordWebSocketApi
    {
        private readonly ClientWebSocket _clientWebSocket;
        private readonly IOptions<AppSettings> _options;

        public DiscordWebSocketApi(ClientWebSocket clientWebSocket, IOptions<AppSettings> options)
        {
            _clientWebSocket = clientWebSocket;
            _options = options;
        }

        public async Task<GatewayPayload> ConnectToSocketApi(string uri)
        {
            await _clientWebSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
            var payload = await RecievePayload();
            if (payload.op == OpCode.Hello)
            {
                return payload;
            }
            else
                throw new Exception("Error");
        }

        private async Task SendPayload(GatewayPayload payload)
        {
            var jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            var json = JsonConvert.SerializeObject(payload, Formatting.Indented, jsonSettings);
            Console.WriteLine(json);
            var bytes = Encoding.UTF8.GetBytes(json);
            await _clientWebSocket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task<GatewayPayload> IdentifyToSocket(string uri)
        {
            try
            {
                GatewayPayload payload = new GatewayPayload();
                payload.op = OpCode.Identify;
                var dataPayload = new IdentifyDataPayload();
                dataPayload.token = _options.Value.BotToken;
                dataPayload.properties = new IdentifyDataPayloadProperties("linux", "my_library", "MyClient");

                payload.d = dataPayload;

                await SendPayload(payload);

                var recievedPayload = await RecievePayload();

                return recievedPayload;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task SendHearthBeat(int wait)
        {
            while (true)
            {
                GatewayPayload payload = new GatewayPayload();
                payload.op = OpCode.HeartBeat;
                payload.d = 251;

                await Task.Delay(wait);
                await SendPayload(payload);

                var response = await RecievePayload();
            }
        }

        private async Task<GatewayPayload> RecievePayload()
        {
            var buffer = new ArraySegment<byte>(new byte[2048]);
            WebSocketReceiveResult result;
            using (var memmoryStream = new MemoryStream())
            {
                do
                {
                    result = await _clientWebSocket.ReceiveAsync(buffer, CancellationToken.None);
                    memmoryStream.Write(buffer.Array, buffer.Offset, result.Count);
                }
                while (!result.EndOfMessage);

                GatewayPayload payload = null;
                if (result.MessageType != WebSocketMessageType.Close)
                {
                    memmoryStream.Seek(0, SeekOrigin.Begin);
                    using (var reader = new StreamReader(memmoryStream, Encoding.UTF8))
                    {
                        payload = JsonConvert.DeserializeObject<GatewayPayload>(await reader.ReadToEndAsync());
                        Console.WriteLine(await reader.ReadToEndAsync());
                    }
                }
                return payload;
            }
        }
    }
}