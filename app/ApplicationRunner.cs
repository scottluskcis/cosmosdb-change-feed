using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Shared.Entities;
using Shared.Services;

namespace App
{
    public interface IApplicationRunner
    {
        Task RunAsync();
    }

    internal class ApplicationRunner : IApplicationRunner
    {
        private readonly ILogger _logger;
        private readonly ICosmosService _service;

        public ApplicationRunner(ICosmosService service, ILogger<ApplicationRunner> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            _logger.LogInformation($"{nameof(ApplicationRunner)} is Running...");

            await SetupPersonEntityAsync();

            _logger.LogInformation($"{nameof(ApplicationRunner)} is finished");
        }

        private async Task SetupPersonEntityAsync()
        {
            _logger.LogDebug($"Setting up {nameof(PersonEntity)}");

            await _service.CreateContainerIfNotExistsAsync<PersonEntity>();
            _logger.LogDebug("Container Created");

            var json = JArray.Parse(await File.ReadAllTextAsync(@"data\PersonData.json"));
            var data = json.ToObject<IEnumerable<PersonEntity>>(); 
            await _service.BulkCreateItemsAsync(data);
            _logger.LogDebug("Sample data added to container"); 

            _logger.LogDebug($"Finished setting up {nameof(PersonEntity)}");
        }
    }
}
