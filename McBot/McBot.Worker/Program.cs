using McBot.Contracts;
using McBot.Core;
using McBot.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace McBot.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    var environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

                    IConfiguration Configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", true, true)
                    .AddJsonFile($"appsettings.{environmentName}.json", true, true)
                    .AddEnvironmentVariables()
                    .Build();

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

                    services.AddClientWebSocket();
                    services.AddSingleton<IDiscordHttpApi, DiscordHttpApi>();
                    services.AddSingleton<IDiscordWebSocketApi, DiscordWebSocketApi>();
                    services.AddTransient(typeof(Bot));
                    services.AddHostedService<Worker>();
                });
    }
}