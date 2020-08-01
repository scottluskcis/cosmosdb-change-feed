using System;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Shared.Configuration;
using Microsoft.Extensions.Options;

namespace app
{
    public class Application
    {
        private readonly ILogger _logger;
        private readonly AppConfiguration _config;

        public Application(IOptions<AppConfiguration> configuration, ILogger<Application> logger)
        {
            _config = configuration.Value;
            _logger = logger;
        }

        internal async Task RunAsync()
        {
            _logger.LogInformation($"{nameof(RunAsync)} - Start");
            
            _logger.LogInformation($"config value: {_config.Value}");

            await Task.Delay(10);

            _logger.LogInformation($"{nameof(RunAsync)} - End");
        }
    }
}