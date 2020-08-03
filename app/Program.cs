using System;
using System.Threading.Tasks;
using App;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Shared.Configuration;
using Shared.Extensions;
using Shared.Services;

namespace app
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var hostBuilder = new HostBuilder()
                .ConfigureHostConfiguration((config) =>
                {
                    config.AddEnvironmentVariables();
                })
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.SetBasePath(Environment.CurrentDirectory);
                    config.AddJsonFile("appsettings.json", optional: false);
                    config.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureLogging((hostContext, config) =>
                {
                    config.AddConsole();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddConfigurationSection<CosmosDbConfiguration>("CosmosDb");

                    services.AddCosmosClient();
                    services.AddTransient<ICosmosService, CosmosService>();
                    
                    services.AddTransient<IApplicationRunner, ApplicationRunner>();
                    services.AddScoped<IHostedService, ApplicationHostedService>();
                });

            await hostBuilder.RunConsoleAsync();
        }

    }
}
