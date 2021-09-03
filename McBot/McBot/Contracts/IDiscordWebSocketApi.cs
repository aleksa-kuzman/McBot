using McBot.Core;
using McBot.Gateway.Payloads;
using System.Threading.Tasks;

namespace McBot.Contracts
{
    public interface IDiscordWebSocketApi
    {
        Task<GatewayHello> ConnectToSocketApi(string uri);

        Task<IdentifyRecieveReadyPayload> IdentifyToSocket(string uri);

        Task<MessageCreated> MessageCreatedEvent();

        public event Respond RespondToCreateMessage;

        Task<VoicePayload> ConnectToVoice(VoiceStateUpdate payload);
    }
}