using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Shared.Entities;
using Shared.Services;

namespace app
{
    public class Application
    {
        private readonly ILogger _logger;
        private readonly ICosmosService<PersonEntity> _service;

        public Application(ICosmosService<PersonEntity> service, ILogger<Application> logger)
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