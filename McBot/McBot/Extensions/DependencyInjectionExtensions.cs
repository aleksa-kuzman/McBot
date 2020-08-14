using Microsoft.Extensions.DependencyInjection;
using System.Net.WebSockets;

namespace McBot.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static void AddClientWebSocket(this IServiceCollection services)
        {
            services.AddTransient(typeof(ClientWebSocket));
        }
    }
}