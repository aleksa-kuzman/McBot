using McBot.Gateway.Payloads;
using System.Threading.Tasks;

namespace McBot.Contracts
{
    public interface IDiscordWebSocketApi
    {
        Task<GatewayPayload> ConnectToSocketApi(string uri);

        Task<GatewayPayload> IdentifyToSocket(string uri);

        Task<GatewayPayload> SendHearthBeat(int wait);
    }
}