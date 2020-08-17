using McBot.Core;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Threading.Tasks;

namespace MC_Server_Starter
{
    public class Program
    {
        private static void Main(string[] args)
        {
            RunKestrel().Wait();

            Console.ReadKey();
        }

        private static async Task RunKestrel()
        {
            var host = ((IWebHostBuilder)new WebHostBuilder())
                .UseKestrel()
                .UseStartup<Startup>()
                .Build();
            var bot = (Bot)host.Services.GetService(typeof(Bot));
            await bot.RunAsync();
            host.Start();
        }
    }
}