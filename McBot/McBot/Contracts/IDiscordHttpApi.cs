using MC_Server_Starter.Gateway.Payloads;
using System.Net.Http;
using System.Threading.Tasks;

namespace McBot.Contracts
{
    public interface IDiscordHttpApi
    {
        public Task<GatewayResource> GetWebSocketBotGateway();

        Task<HttpResponseMessage> CreateMessage();
    }
}