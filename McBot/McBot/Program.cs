using MC_Server_Starter.Gateway;
using McBot;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
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
            //await SendMessage();
        }

       /* private static async Task SendMessage()
        {
            using (HttpClient Client = new HttpClient())
            {
                Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bot", "NzQxMzUyOTczMTAzMjY3OTgy.Xy2Uwg.OSMLFuKsMX399XwkW6AiA4KXURw");
                Client.DefaultRequestHeaders.Add("User-Agent", "myTestAp | this is a");

                var json = new Message("Hey man hi, server up", false);
                HttpContent content = new StringContent(JsonConvert.SerializeObject(json), Encoding.UTF8, "application/json");

                HttpResponseMessage message = await Client.PostAsync("https://discord.com/api/channels/741382227249856544/messages?payload_json", content);
                var somethingElse = JsonConvert.DeserializeObject<GatewayResource>(await message.Content.ReadAsStringAsync());
                Console.WriteLine(somethingElse);
            }
        }*/

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