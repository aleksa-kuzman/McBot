using McBot;
using McBot.Contracts;
using McBot.Core;
using McBot.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MC_Server_Starter
{
    public class Startup
    {
        public static IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration);
            services.AddHttpClient("DiscordHttpApi", c =>
            {
                c.BaseAddress = new Uri("https://discord.com/api/");
                c.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bot", Configuration.GetSection("BotToken").Value);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            services.AddHttpClient("WhatisMyIpApi", c =>
            {
                c.BaseAddress = new Uri("http://ipv4.icanhazip.com/");
            });
            services.AddTransient<IDiscordHttpApi, DiscordHttpApi>();
            services.AddTransient<IDiscordWebSocketApi, DiscordWebSocketApi>();
            services.AddTransient(typeof(Bot));
            services.AddClientWebSocket();
            services.AddMvcCore();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();
        }
    }
}