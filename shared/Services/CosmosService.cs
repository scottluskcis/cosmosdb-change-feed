using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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

        public async Task<TEntity> CreateItemAsync(TEntity item, CancellationToken cancellationToken = default)
        {
            var container = await GetContainerAsync(cancellationToken);

            var itemResponse = await container.CreateItemAsync(item, cancellationToken: cancellationToken);
            _logger.LogItemResponse(itemResponse);

            return itemResponse.Resource;
        }

        public async Task<TEntity> ReadItemAsync(string id, string partitionKey, CancellationToken cancellationToken = default)
        {
            var container = await GetContainerAsync(cancellationToken);

            var pk = new PartitionKey(partitionKey);
            _logger.LogPartitionKey(pk, container);
            
            var itemResponse = await container.ReadItemAsync<TEntity>(id, pk, cancellationToken: cancellationToken); 
            _logger.LogItemResponse(itemResponse);

            return itemResponse.Resource;
        }

        public Task<IEnumerable<TEntity>> ReadItemsAsync(string id, string partitionKey, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<TEntity> ReplaceItemAsync(TEntity item, CancellationToken cancellationToken = default)
        {
            var container = await GetContainerAsync(cancellationToken);

            var pk = item.GetPartitionKey();
            _logger.LogPartitionKey(pk, container);

            var itemResponse = await container.ReplaceItemAsync(item, item.Id, pk, cancellationToken: cancellationToken);
            _logger.LogItemResponse(itemResponse);
            
            return itemResponse.Resource;
        }

        public async Task<TEntity> DeleteItemAsync(string id, string partitionKey, CancellationToken cancellationToken = default)
        {
            var container = await GetContainerAsync(cancellationToken);
            
            var pk = new PartitionKey(partitionKey);
            _logger.LogPartitionKey(pk, container);

            var itemResponse = await container.DeleteItemAsync<TEntity>(id, pk, cancellationToken: cancellationToken);
            _logger.LogItemResponse(itemResponse);
            
            return itemResponse.Resource;
        }

        private async Task<Container> GetContainerAsync(CancellationToken cancellationToken = default)
        {
            if (_container != null)
                return _container;
             
            var database = await CreateDatabaseAsync(cancellationToken); 
            var props = typeof(TEntity).GetContainerProperties();
            
            var containerResponse = await database.CreateContainerIfNotExistsAsync(props, cancellationToken: cancellationToken);
            _logger.LogContainerResponse(containerResponse);
            
            return _container ??= containerResponse.Container;
        }

        private async Task<Database> CreateDatabaseAsync(CancellationToken cancellationToken = default)
        {
            if (_database != null)
                return _database;
            
            var databaseResponse = await _client.CreateDatabaseIfNotExistsAsync(_config.DatabaseId, cancellationToken: cancellationToken);
            _logger.LogDatabaseResponse(databaseResponse);
             
            return _database ??= databaseResponse.Database;
        }
    }
}