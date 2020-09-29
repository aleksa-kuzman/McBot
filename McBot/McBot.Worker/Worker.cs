using McBot.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace McBot.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly Bot _bot;

        public Worker(ILogger<Worker> logger, Bot bot)
        {
            _logger = logger;
            _bot = bot;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _bot.RunAsync();

            while (true)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    await _bot.KillServer();
                }
            }
        }
    }
}