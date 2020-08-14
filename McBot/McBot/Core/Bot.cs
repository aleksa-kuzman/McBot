using McBot.Contracts;
using System.Threading.Tasks;

namespace McBot.Core
{
    public class Bot
    {
        private readonly IDiscordHttpApi _discordApi;

        public async Task RunAsync()
        {
        }

        public Bot(IDiscordHttpApi discordApi)
        {
            _discordApi = discordApi;
        }
    }
}