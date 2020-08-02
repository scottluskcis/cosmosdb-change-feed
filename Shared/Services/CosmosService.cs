using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Shared.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Entities;
using Shared.Extensions;

namespace Shared.Services
{
    public class CosmosService<TEntity> : ICosmosService<TEntity>
        where TEntity : BaseEntity
    {
        private readonly CosmosClient _client;
        private readonly CosmosDbConfiguration _config;
        private readonly ILogger _logger;

        private Database _database;
        private Container _container;

        public CosmosService(CosmosClient client, IOptions<CosmosDbConfiguration> configuration, ILogger<CosmosService<TEntity>> logger)
        {
            _client = client;
            _config = configuration.Value;
            _logger = logger;
        }

        public async Task<TEntity> CreateItemAsync(TEntity item)
        {
            var container = await GetContainerAsync();

            var itemResponse = await container.CreateItemAsync(item);
            _logger.LogItemResponse(itemResponse);

            return itemResponse.Resource;
        }

        public async Task<TEntity> ReadItemAsync(string id, string partitionKeyValue)
        {
            var container = await GetContainerAsync();

            var partitionKey = new PartitionKey(partitionKeyValue);
            _logger.LogPartitionKey(partitionKey, container);
            
            var itemResponse = await container.ReadItemAsync<TEntity>(id, partitionKey); 
            _logger.LogItemResponse(itemResponse);

            return itemResponse.Resource;
        }

        public async Task<TEntity> ReplaceItemAsync(TEntity item)
        {
            var container = await GetContainerAsync();

            var partitionKey = item.GetPartitionKey();
            _logger.LogPartitionKey(partitionKey, container);

            var itemResponse = await container.ReplaceItemAsync(item, item.Id, partitionKey);
            _logger.LogItemResponse(itemResponse);
            
            return itemResponse.Resource;
        }

        public async Task<TEntity> DeleteItemAsync(string id, string partitionKeyValue)
        {
            var container = await GetContainerAsync();
            
            var partitionKey = new PartitionKey(partitionKeyValue);
            _logger.LogPartitionKey(partitionKey, container);

            var itemResponse = await container.DeleteItemAsync<TEntity>(id, partitionKey);
            _logger.LogItemResponse(itemResponse);
            
            return itemResponse.Resource;
        }

        private async Task<Container> GetContainerAsync()
        {
            if (_container != null)
                return _container;
             
            var database = await CreateDatabaseAsync(); 
            var props = typeof(TEntity).GetContainerProperties();
            
            var containerResponse = await database.CreateContainerIfNotExistsAsync(props);
            _logger.LogContainerResponse(containerResponse);
            
            return _container ??= containerResponse.Container;
        }

        private async Task<Database> CreateDatabaseAsync()
        {
            if (_database != null)
                return _database;
            
            var databaseResponse = await _client.CreateDatabaseIfNotExistsAsync(_config.DatabaseId);
            _logger.LogDatabaseResponse(databaseResponse);
             
            return _database ??= databaseResponse.Database;
        }
    }
}