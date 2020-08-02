﻿using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Options;
using Shared.Configuration;
using Shared.Extensions;
using Shared.Services;

namespace app
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions<CosmosDbConfiguration>()
                        .Bind(config.GetSection("CosmosDb"));

                    services.AddLogging(configure => configure.AddConsole());
                    services.AddTransient<Application>();

                    services.AddCosmosClient();
                    services.AddScoped(typeof(ICosmosService<>), typeof(CosmosService<>));
                });

            var host = builder.Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                try
                {
                    var appService = services.GetRequiredService<Application>();
                    await appService.RunAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Occurred: {ex}");
                }
            }

            Console.ReadKey();
        }

    }
}
