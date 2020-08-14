using MC_Server_Starter.Gateway;
using McBot.Gateway;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;
using System.IO;
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
            var client = await ConnectToSocketApi(gateway.Url + "?v=6&encoding=json");
            // await IdentifyToSocket(gateway.Url, client);

            //await SendMessage();
        }

        /* private static async Task IdentifyToSocket(string uri, ClientWebSocket clientWebSocket)
          {
              try
              {
                  GatewayPayload payload = new GatewayPayload();
                  payload.op = 2;
                  var dataPayload = new IdentifyDataPayload();
                  dataPayload.Token = "NzQxMzUyOTczMTAzMjY3OTgy.Xy2Uwg.OSMLFuKsMX399XwkW6AiA4KXURw";
                  dataPayload.Properties = new IdentifyDataPayloadProperties("linux", "my_library", "MyClient");

                  payload.d = dataPayload;

                  var jsonSettings = new JsonSerializerSettings();
                  jsonSettings.NullValueHandling = NullValueHandling.Ignore;
                  var json = JsonConvert.SerializeObject(payload, Formatting.Indented, jsonSettings);
                  var bytes = Encoding.UTF8.GetBytes(json);
                  await clientWebSocket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);

                  WebSocketReceiveResult result;
                  var buffer = new ArraySegment<byte>(new byte[2048]);
                  using (var memmoryStream = new MemoryStream())
                  {
                      do
                      {
                          result = await clientWebSocket.ReceiveAsync(buffer, CancellationToken.None);
                          memmoryStream.Write(buffer.Array, buffer.Offset, result.Count);
                      }
                      while (!result.EndOfMessage);

                      if (result.MessageType != WebSocketMessageType.Close)
                      {
                          memmoryStream.Seek(0, SeekOrigin.Begin);
                          using (var reader = new StreamReader(memmoryStream, Encoding.UTF8))
                          {
                              var recieve = JsonConvert.DeserializeObject<GatewayPayload>(await reader.ReadToEndAsync());
                              Console.WriteLine(recieve);
                          }
                      }
                  }
              }
              catch (Exception ex)
              {
                  throw;
              }
          }*/

        private static async Task<ClientWebSocket> ConnectToSocketApi(string uri)
        {
            try
            {
                ClientWebSocket socketClient = new ClientWebSocket();
                await socketClient.ConnectAsync(new Uri(uri), CancellationToken.None);
                var buffer = new ArraySegment<byte>(new byte[2048]);

                WebSocketReceiveResult result;
                using (var memmoryStream = new MemoryStream())
                {
                    do
                    {
                        result = await socketClient.ReceiveAsync(buffer, CancellationToken.None);
                        memmoryStream.Write(buffer.Array, buffer.Offset, result.Count);
                    }
                    while (!result.EndOfMessage);

                    if (result.MessageType != WebSocketMessageType.Close)
                    {
                        memmoryStream.Seek(0, SeekOrigin.Begin);
                        using (var reader = new StreamReader(memmoryStream, Encoding.UTF8))
                        {
                            var payload = JsonConvert.DeserializeObject<GatewayPayload>(await reader.ReadToEndAsync());
                            Console.WriteLine(await reader.ReadToEndAsync());
                        }
                    }
                }

                return socketClient;
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