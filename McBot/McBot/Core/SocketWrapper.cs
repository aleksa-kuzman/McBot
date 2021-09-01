using McBot.Gateway.Payloads;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace McBot.Core
{
    public class SocketWrapper
    {
        private readonly ClientWebSocket _socket;

        public SocketWrapper(ClientWebSocket socket)
        {
            _socket = socket;
        }

        private async Task SendPayload<T>(T payload) where T : Payload
        {
            try
            {
                var jsonSettings = new JsonSerializerSettings();
                jsonSettings.NullValueHandling = NullValueHandling.Ignore;
                var json = JsonConvert.SerializeObject(payload, Formatting.Indented, jsonSettings);
                Console.WriteLine(json);
                var bytes = Encoding.UTF8.GetBytes(json);
                await _socket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<T> ConnectToVoiceSocket<T>(string uri) where T : Payload
        {
            await _socket.ConnectAsync(new Uri(uri), CancellationToken.None);
            var payload = await RecieveVoicePayload<T>();

            return JsonConvert.DeserializeObject<T>(payload.d.ToString());
        }

        private async Task SendHearthBeat<T>(T payload, int wait) where T : Payload, new()
        {
            while (true)
            {
                payload.d = new { heartbeat_interval = wait };

                await Task.Delay(wait);
                await SendPayload(payload);
            }
        }

        private async Task<T> RecieveVoicePayload<T>()
        {
            var buffer = new ArraySegment<byte>(new byte[2048]);
            WebSocketReceiveResult result;
            using (var memmoryStream = new MemoryStream())
            {
                do
                {
                    result = await _socket.ReceiveAsync(buffer, CancellationToken.None);
                    memmoryStream.Write(buffer.Array, buffer.Offset, result.Count);
                }
                while (!result.EndOfMessage);

                T payload = default;
                if (result.MessageType != WebSocketMessageType.Close)
                {
                    memmoryStream.Seek(0, SeekOrigin.Begin);
                    using (var reader = new StreamReader(memmoryStream, Encoding.UTF8))
                    {
                        payload = JsonConvert.DeserializeObject<T>(await reader.ReadToEndAsync());
                        Console.WriteLine(await reader.ReadToEndAsync());
                    }
                }
                return payload;
            }
        }
    }
}