using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.Configuration.Json;
using Shared.Configuration;

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
                    services.AddOptions<AppConfiguration>()
                        .Bind(config.GetSection("App"));

                    services.AddLogging(configure => configure.AddConsole());
                    services.AddTransient<Application>();
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

           // Console.WriteLine("Press any key to quit...");
           Console.ReadKey();
        }

    }
}
