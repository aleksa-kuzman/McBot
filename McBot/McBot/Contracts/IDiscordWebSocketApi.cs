using System.Net.WebSockets;
using System.Threading.Tasks;

namespace McBot.Contracts
{
    public interface IDiscordWebSocketApi
    {
        Task<bool> ConnectToSocketApi(string uri);

        Task IdentifyToSocket(string uri, ClientWebSocket clientWebSocket);
    }
}