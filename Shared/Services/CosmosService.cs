using Microsoft.Azure.Cosmos;
using Shared.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Shared.Services
{
    public class CosmosService : ICosmosService
    {
        private readonly CosmosClient _client;
        private readonly ILogger _logger;
        private readonly CosmosDbConfiguration _config;

        public CosmosService(CosmosClient client, IOptions<CosmosDbConfiguration> configuration, ILogger<CosmosService> logger)
        {
            _client = client;
            _config = configuration.Value;
            _logger = logger;
        }
    }
}