using McBot.Contracts;
using McBot.Core;
using McBot.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MC_Server_Starter
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore();
            services.AddHttpClient("DiscordHttpApi", c =>
            {
                c.BaseAddress = new Uri("https://discord.com/api");
                c.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bot", "NzQxMzUyOTczMTAzMjY3OTgy.Xy2Uwg.OSMLFuKsMX399XwkW6AiA4KXURw");
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            services.AddTransient<IDiscordHttpApi, DiscordHttpApi>();
            services.AddTransient<IDiscordWebSocketApi, DiscordWebSocketApi>();
            services.AddTransient(typeof(Bot));
            services.AddClientWebSocket();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();
        }
    }
}