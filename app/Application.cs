using System;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace app
{
    public class Application
    {
        private readonly ILogger _logger;

        public Application(ILogger<Application> logger)
        {
            _logger = logger;
        }

        internal async Task RunAsync()
        {
            _logger.LogInformation($"{nameof(RunAsync)} - Start");
            
            await Task.Delay(10);

            _logger.LogInformation($"{nameof(RunAsync)} - End");
        }
    }
}