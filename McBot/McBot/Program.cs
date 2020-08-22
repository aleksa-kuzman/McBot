using McBot.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
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
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{environmentName}.json", true, true)
                .AddEnvironmentVariables()
                .Build();

            var host = ((IWebHostBuilder)new WebHostBuilder())
                .UseKestrel()
                .UseConfiguration(configuration)
                .UseStartup<Startup>()
                .Build();
            var bot = (Bot)host.Services.GetService(typeof(Bot));
            await bot.RunAsync();
            host.Start();
        }
    }
}