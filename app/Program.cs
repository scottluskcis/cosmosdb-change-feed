using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace app
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
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

                    Console.WriteLine("Press Enter to Quit...");
                    Console.ReadKey();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Occurred: {ex}");
                }
            }
        }
    }
}
