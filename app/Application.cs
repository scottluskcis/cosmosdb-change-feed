using System;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Shared.Configuration;
using Microsoft.Extensions.Options;
using Shared.Services;

namespace app
{
    public class Application
    {
        private readonly ILogger _logger;
        private readonly ICosmosService _service;

        public Application(ICosmosService service, ILogger<Application> logger)
        {
            _service = service;
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