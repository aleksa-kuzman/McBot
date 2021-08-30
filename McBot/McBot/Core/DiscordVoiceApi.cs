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
    internal class DiscordVoiceApi
    {
        private readonly ClientWebSocket _voiceWebSocket;
        private readonly IOptions<AppSettings> _options;

        public DiscordVoiceApi(ClientWebSocket voiceWebSocket, IOptions<AppSettings> options)
        {
            _voiceWebSocket = voiceWebSocket;
            _options = options;
        }

        private async Task SendVoicePayload(VoicePayload payload)
        {
            try
            {
                var jsonSettings = new JsonSerializerSettings();
                jsonSettings.NullValueHandling = NullValueHandling.Ignore;
                var json = JsonConvert.SerializeObject(payload, Formatting.Indented, jsonSettings);
                Console.WriteLine(json);
                var bytes = Encoding.UTF8.GetBytes(json);
                await _voiceWebSocket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task SendVoiceHearthBeat(int wait)
        {
            while (true)
            {
                VoicePayload payload = new VoicePayload();
                payload.op = VoiceOpCode.Heartbeat;
                payload.d = new { heartbeat_interval = wait };

                await Task.Delay(wait);
                await SendVoicePayload(payload);
            }
        }

        public async Task<VoiceHello> ConnectToVoiceSocket(string uri)
        {
            await _voiceWebSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
            var payload = await RecieveVoicePayload();
            if (payload.op == VoiceOpCode.Hello)
            {
                return payload.VoiceHello;
            }
            else
                throw new NullReferenceException("Gateway hello is null something is  not right");
        }

        private async Task<VoicePayload> RecieveVoicePayload()
        {
            var buffer = new ArraySegment<byte>(new byte[2048]);
            WebSocketReceiveResult result;
            using (var memmoryStream = new MemoryStream())
            {
                do
                {
                    result = await _voiceWebSocket.ReceiveAsync(buffer, CancellationToken.None);
                    memmoryStream.Write(buffer.Array, buffer.Offset, result.Count);
                }
                while (!result.EndOfMessage);

                VoicePayload payload = null;
                if (result.MessageType != WebSocketMessageType.Close)
                {
                    memmoryStream.Seek(0, SeekOrigin.Begin);
                    using (var reader = new StreamReader(memmoryStream, Encoding.UTF8))
                    {
                        payload = JsonConvert.DeserializeObject<VoicePayload>(await reader.ReadToEndAsync());
                        Console.WriteLine(await reader.ReadToEndAsync());
                    }
                }
                return payload;
            }
        }
    }
}