using MC_Server_Starter.Gateway;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace MC_Server_Starter
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            RunKestrel();
            RunAsync().Wait();

            Console.ReadKey();
        }

        private static void RunKestrel()
        {
            var host = ((IWebHostBuilder)new WebHostBuilder())
                .UseKestrel()
                .UseStartup<Startup>()
                .Build();

            host.Start();
        }

        private static async Task RunAsync()
        {
            var gateway = await GetWebSocketGateway();
            await SendHello(gateway.Url);
        }

        private static async Task SendHello(string uri)
        {
            try
            {
                ClientWebSocket socketClient = new ClientWebSocket();
                await socketClient.ConnectAsync(new Uri(uri), CancellationToken.None);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static async Task<GatewayResource> GetWebSocketGateway()
        {
            using (HttpClient Client = new HttpClient())
            {
                Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bot", "NzQxMzUyOTczMTAzMjY3OTgy.Xy2Uwg.OSMLFuKsMX399XwkW6AiA4KXURw");
                Client.DefaultRequestHeaders.Add("User-Agent", "myTestAp | this is a");

                HttpResponseMessage message = await Client.GetAsync("https://discord.com/api/gateway/bot");
                var somethingElse = JsonConvert.DeserializeObject<GatewayResource>(await message.Content.ReadAsStringAsync());
                Console.WriteLine(somethingElse);

                return somethingElse;
            }
        }
    }
}