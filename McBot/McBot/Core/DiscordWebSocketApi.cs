using McBot.Contracts;
using McBot.Gateway.Payloads;
using Microsoft.Extensions.Options;
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace McBot.Core
{
    public delegate Task Respond(MessageCreated message);

    public class DiscordWebSocketApi : IDiscordWebSocketApi
    {
        private readonly SocketWrapper _wrapper;
        private readonly ClientWebSocket _voiceWebSocket;
        private readonly IOptions<AppSettings> _options;

        public event Respond RespondToCreateMessage;

        public DiscordWebSocketApi(SocketWrapper wrapper, IOptions<AppSettings> options, ClientWebSocket voiceWebSocket)
        {
            _wrapper = wrapper;
            _options = options;
            _voiceWebSocket = voiceWebSocket;
        }

        public async Task<GatewayHello> ConnectToSocketApi(string uri)
        {
            try
            {
                var gatewayHello = await _wrapper.ConnectToSocket<GatewayPayload>(uri);
                if (gatewayHello.op != OpCodeEnumeration.Hello)
                {
                    // TODO: handle exceptions better
                    throw new Exception("Need  to implemetn exception handling");
                }

                GatewayPayload heartBeatPayload = new GatewayPayload();
                heartBeatPayload.op = OpCodeEnumeration.HeartBeat;
                heartBeatPayload.d = 251;

                var connected = gatewayHello.GatewayHello;

                if (connected == null)
                {
                    // TODO: HandleExceptions
                    throw new Exception("Something 12345");
                }
                _ = _wrapper.SendHearthBeat(heartBeatPayload, gatewayHello.GatewayHello.heartbeat_interval);

                return gatewayHello.GatewayHello;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task SendVoiceStateUpdate(VoiceStateUpdate payload)
        {
            var gatewayPayload = new GatewayPayload();
            gatewayPayload.op = OpCodeEnumeration.VoiceStateUpdate;
            gatewayPayload.d = payload;
            await _wrapper.SendPayload<GatewayPayload>(gatewayPayload);
        }

        private VoicePayload GetVoiceServerInfo(VoiceStateUpdate voiceStateUpdate, VoiceServerUpdate voiceServerUpdate)
        {
            VoicePayload voiceIdentifyPayload = new VoicePayload();
            voiceIdentifyPayload.op = VoiceOpCodeEnumeration.Identify;
            VoiceIdentify voiceIdentify = new VoiceIdentify(voiceServerUpdate.GuildId, voiceStateUpdate.UserId, voiceStateUpdate.SesssionId, voiceServerUpdate.Token, voiceServerUpdate.Endpoint);
            voiceIdentifyPayload.d = voiceIdentify;

            return voiceIdentifyPayload;
        }

        public async Task<VoicePayload> ConnectToVoice(VoiceStateUpdate payload)
        {
            try
            {
                await SendVoiceStateUpdate(payload);

                VoiceStateUpdate voiceStateUpdate = await _wrapper.WaitForAsync<VoiceStateUpdate>();
                VoiceServerUpdate voiceServerUpdate = await _wrapper.WaitForAsync<VoiceServerUpdate>();
                var voiceIdentifyPayload = GetVoiceServerInfo(voiceStateUpdate, voiceServerUpdate);

                return voiceIdentifyPayload;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                throw;
            }
        }

        public async Task<IdentifyRecieveReadyPayload> IdentifyToSocket(string uri)
        {
            try
            {
                GatewayPayload payload = new GatewayPayload();
                payload.op = OpCodeEnumeration.Identify;
                var dataPayload = new IdentifyDataPayload();
                dataPayload.token = _options.Value.BotToken;
                dataPayload.properties = new IdentifyDataPayloadProperties("linux", "my_library", "MyClient");

                payload.d = dataPayload;

                await _wrapper.SendPayload(payload);

                var recievedPayload = await _wrapper.RecievePayload<GatewayPayload>();

                if (recievedPayload.op == OpCodeEnumeration.InvalidSession)
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

        public async Task<MessageCreated> MessageCreatedEvent()
        {
            try
            {
                var gatewayPayload = await _wrapper.RecievePayload<GatewayPayload>();

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
    }
}