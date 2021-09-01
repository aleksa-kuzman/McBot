using McBot.Gateway.Payloads;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace McBot.Core
{
    public class SocketWrapper
    {
        private readonly ClientWebSocket _socket;
        private readonly ILogger<SocketWrapper> _logger;

        public SocketWrapper(ClientWebSocket socket, ILogger<SocketWrapper> logger)
        {
            _socket = socket;
            _logger = logger;
        }

        public async Task SendPayload<T>(T payload) where T : Payload
        {
            try
            {
                var jsonSettings = new JsonSerializerOptions { IgnoreNullValues = true };
                var json = JsonSerializer.Serialize(payload, jsonSettings);

                Console.WriteLine(json);
                var bytes = Encoding.UTF8.GetBytes(json);
                await _socket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<T> ConnectToSocket<T>(string uri) where T : Payload
        {
            await _socket.ConnectAsync(new Uri(uri), CancellationToken.None);
            var payload = await RecievePayload<T>();

            return payload;
        }

        public async Task SendHearthBeat<T>(T payload, int wait) where T : Payload, new()
        {
            while (true)
            {
                payload.d = new { heartbeat_interval = wait };

                await Task.Delay(wait);
                await SendPayload(payload);
            }
        }

        public async Task<T> WaitForAsync<T>()
        {
            T waitedPayload = default;
            while (waitedPayload is null)
            {
                waitedPayload = await GetPayload<T>();
            }

            return waitedPayload;
        }

        public async Task<T> GetPayload<T>()
        {
            try
            {
                var gatewayPayload = await RecievePayload<GatewayPayload>();

                if (gatewayPayload.GetPayloadType() == typeof(T))
                {
                    return gatewayPayload.GetPayload<T>();
                }
                else return default(T);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<T> RecievePayload<T>()
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
                        payload = JsonSerializer.Deserialize<T>(await reader.ReadToEndAsync());
                        Console.WriteLine(await reader.ReadToEndAsync());
                    }
                }
                return payload;
            }
        }
    }
}