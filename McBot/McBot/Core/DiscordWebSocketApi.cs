using McBot.Contracts;
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
    public delegate Task Respond(MessageCreated message);

    public class DiscordWebSocketApi : IDiscordWebSocketApi
    {
        private readonly ClientWebSocket _clientWebSocket;
        private readonly ClientWebSocket _voiceWebSocket;
        private readonly IOptions<AppSettings> _options;

        public event Respond RespondToCreateMessage;

        public DiscordWebSocketApi(ClientWebSocket clientWebSocket, IOptions<AppSettings> options, ClientWebSocket voiceWebSocket)
        {
            _clientWebSocket = clientWebSocket;
            _options = options;
            _voiceWebSocket = voiceWebSocket;
        }

        public async Task<GatewayHello> ConnectToSocketApi(string uri)
        {
            await _clientWebSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
            var payload = await RecievePayload();
            if (payload.op == OpCode.Hello)
            {
                return payload.GatewayHello;
            }
            else
                throw new NullReferenceException("Gateway hello is null something is  not right");
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

        private async Task SendPayload(GatewayPayload payload, ClientWebSocket clientWebSocket)
        {
            try
            {
                var jsonSettings = new JsonSerializerSettings();
                jsonSettings.NullValueHandling = NullValueHandling.Ignore;
                var json = JsonConvert.SerializeObject(payload, Formatting.Indented, jsonSettings);
                Console.WriteLine(json);
                var bytes = Encoding.UTF8.GetBytes(json);
                await clientWebSocket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception ex)
            {
                throw;
            }
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

        private async Task<T> WaitForAsync<T>()
        {
            T waitedPayload = default;
            while (waitedPayload is null)
            {
                waitedPayload = await GetPayload<T>();
            }

            return waitedPayload;
        }

        private async Task SendVoiceStateUpdate(VoiceStateUpdate payload)
        {
            var gatewayPayload = new GatewayPayload();
            gatewayPayload.op = OpCode.VoiceStateUpdate;
            gatewayPayload.d = payload;
            await SendPayload(gatewayPayload, _clientWebSocket);
        }

        private VoicePayload GetVoiceServerInfo(VoiceStateUpdate voiceStateUpdate, VoiceServerUpdate voiceServerUpdate)
        {
            VoicePayload voiceIdentifyPayload = new VoicePayload();
            voiceIdentifyPayload.op = VoiceOpCode.Identify;
            VoiceIdentify voiceIdentify = new VoiceIdentify(voiceServerUpdate.GuildId, voiceStateUpdate.UserId, voiceStateUpdate.SesssionId, voiceServerUpdate.Token, voiceServerUpdate.Endpoint);
            voiceIdentifyPayload.d = voiceIdentify;

            return voiceIdentifyPayload;
        }

        public async Task ConnectToVoice(VoiceStateUpdate payload)
        {
            try
            {
                await SendVoiceStateUpdate(payload);

                VoiceStateUpdate voiceStateUpdate = await WaitForAsync<VoiceStateUpdate>();
                VoiceServerUpdate voiceServerUpdate = await WaitForAsync<VoiceServerUpdate>();
                var voiceIdentifyPayload = GetVoiceServerInfo(voiceStateUpdate, voiceServerUpdate);

                await DoVoicePart(voiceIdentifyPayload);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                throw;
            }
        }

        private async Task DoVoicePart(VoicePayload payload)
        {
            var hello = await ConnectToVoiceSocket(((VoiceIdentify)payload.d).Endpoint);

            await SendVoicePayload(payload);
            var smt = await RecieveVoicePayload();

            _ = SendVoiceHearthBeat((int)hello.heartbeat_interval);
        }

        public async Task<IdentifyRecieveReadyPayload> IdentifyToSocket(string uri)
        {
            try
            {
                GatewayPayload payload = new GatewayPayload();
                payload.op = OpCode.Identify;
                var dataPayload = new IdentifyDataPayload();
                dataPayload.token = _options.Value.BotToken;
                dataPayload.properties = new IdentifyDataPayloadProperties("linux", "my_library", "MyClient");

                payload.d = dataPayload;

                await SendPayload(payload, _clientWebSocket);

                var recievedPayload = await RecievePayload();
                if (recievedPayload.op == OpCode.InvalidSession)
                {
                    throw new System.Exception("API RETURNED OPCODE 9");
                }

                return recievedPayload.Ready;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<T> GetPayload<T>()
        {
            try
            {
                var gatewayPayload = await RecievePayload();

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

        public async Task<MessageCreated> MessageCreatedEvent()
        {
            try
            {
                var gatewayPayload = await RecievePayload();

                if (gatewayPayload.MessageCreated != null)
                {
                    await OnMessageCreation(gatewayPayload.MessageCreated);
                    return gatewayPayload.MessageCreated;
                }
                else return null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        protected virtual async Task OnMessageCreation(MessageCreated message)
        {
            await RespondToCreateMessage?.Invoke(message);
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

        public async Task SendHearthBeat(int wait)
        {
            while (true)
            {
                GatewayPayload payload = new GatewayPayload();
                payload.op = OpCode.HeartBeat;
                payload.d = 251;

                await Task.Delay(wait);
                await SendPayload(payload, _clientWebSocket);
            }
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