using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Shared.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Entities;
using Shared.Extensions;

namespace Shared.Services
{
    public class CosmosService : ICosmosService
    {
        private readonly CosmosClient _client;
        private readonly CosmosDbConfiguration _config;
        private readonly ILogger _logger;

        public CosmosService(CosmosClient client, IOptions<CosmosDbConfiguration> configuration, ILogger<CosmosService> logger)
        {
            _client = client;
            _config = configuration.Value;
            _logger = logger;
        }

        public async Task<TEntity> CreateItemAsync<TEntity>(
            TEntity item, 
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity
        {
            var container = GetContainer<TEntity>();

            var itemResponse = await container.CreateItemAsync(item, cancellationToken: cancellationToken);
            _logger.LogItemResponse(itemResponse);

            var result = itemResponse.Resource;
            return result;
        }

        public async Task<TEntity> ReadItemAsync<TEntity>(
            string id, 
            string partitionKey,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity
        {
            var container = GetContainer<TEntity>();

            var pk = new PartitionKey(partitionKey);
            _logger.LogPartitionKey(pk, container);
            
            var itemResponse = await container.ReadItemAsync<TEntity>(id, pk, cancellationToken: cancellationToken); 
            _logger.LogItemResponse(itemResponse);

            var result = itemResponse.Resource;
            return result;
        }

        public async Task<TEntity> ReplaceItemAsync<TEntity>(
            TEntity item, 
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity
        {
            var container = GetContainer<TEntity>();

            var pk = item.GetPartitionKey();
            _logger.LogPartitionKey(pk, container);

            var itemResponse = await container.ReplaceItemAsync(item, item.Id, pk, cancellationToken: cancellationToken);
            _logger.LogItemResponse(itemResponse);
            
            var result = itemResponse.Resource;
            return result;
        }

        public async Task<TEntity> DeleteItemAsync<TEntity>(
            string id, 
            string partitionKey, 
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity
        {
            var container = GetContainer<TEntity>();
            
            var pk = new PartitionKey(partitionKey);
            _logger.LogPartitionKey(pk, container);

            var itemResponse = await container.DeleteItemAsync<TEntity>(id, pk, cancellationToken: cancellationToken);
            _logger.LogItemResponse(itemResponse);
            
            var result = itemResponse.Resource;
            return result;
        }
         
        public async Task<IEnumerable<TEntity>> ReadItemsAsync<TEntity>(
            Expression<Func<TEntity, bool>> predicate = null, 
            string partitionKey = "", 
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity
        {
            var container = GetContainer<TEntity>();

            var requestOptions = new QueryRequestOptions();

            if (!string.IsNullOrEmpty(partitionKey))
            {
                requestOptions.PartitionKey = new PartitionKey(partitionKey);
                _logger.LogPartitionKey(requestOptions.PartitionKey.Value, container);
            }

            if (_config.ConsistencyLevel.HasValue)
            {
                requestOptions.ConsistencyLevel = _config.ConsistencyLevel;
                _logger.LogInformation("ConsistencyLevel: {ConsistencyLevel}", requestOptions.ConsistencyLevel);
            }
             
            IQueryable<TEntity> query = container.GetItemLinqQueryable<TEntity>(requestOptions: requestOptions);

            if (predicate != null)
            {
                query = query.Where(predicate);
                _logger.LogDebug("Predicate has been applied to ItemLinqQueryable");
            }

            var result = new List<TEntity>();

            using var feedIterator = query.ToFeedIterator();
            while (feedIterator.HasMoreResults)
            {
                var feedResponse = await feedIterator.ReadNextAsync(cancellationToken);
                _logger.LogFeedResponse(feedResponse);
                result.AddRange(feedResponse);
            }

            return result;
        }

        public async Task<IEnumerable<TEntity>> QueryItemsAsync<TEntity>(
            string sql, 
            string partitionKey = "", 
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity
        {
            var container = GetContainer<TEntity>();

            var requestOptions = new QueryRequestOptions();

            if (!string.IsNullOrEmpty(partitionKey))
            {
                requestOptions.PartitionKey = new PartitionKey(partitionKey);
                _logger.LogPartitionKey(requestOptions.PartitionKey.Value, container);
            }

            if (_config.ConsistencyLevel.HasValue)
            {
                requestOptions.ConsistencyLevel = _config.ConsistencyLevel;
                _logger.LogInformation("ConsistencyLevel: {ConsistencyLevel}", requestOptions.ConsistencyLevel);
            }

            var queryDefinition = new QueryDefinition(sql);

            var result = new List<TEntity>();

            using var feedIterator = container.GetItemQueryIterator<TEntity>(
                queryDefinition, 
                requestOptions: requestOptions);
            
            while (feedIterator.HasMoreResults)
            {
                var feedResponse = await feedIterator.ReadNextAsync(cancellationToken);
                _logger.LogFeedResponse(feedResponse);
                result.AddRange(feedResponse);
            }

            return result;
        }

        private Container GetContainer<TEntity>()
            where TEntity : BaseEntity
        {
            var database = GetDatabase();
            var props = typeof(TEntity).GetContainerProperties();

            var container = database.GetContainer(props.Id);
            return container;
        }

        private Database GetDatabase()
        {
            var database = _client.GetDatabase(_config.DatabaseId);
            return database;
        }

        public async Task<Container> CreateContainerIfNotExistsAsync<TEntity>(CancellationToken cancellationToken = default)
            where TEntity : BaseEntity
        {
            var database = await CreateDatabaseIfNotExistsAsync(_config.DatabaseId, cancellationToken); 
            var props = typeof(TEntity).GetContainerProperties();

            var containerResponse = await database.CreateContainerIfNotExistsAsync(props, cancellationToken: cancellationToken);
            _logger.LogContainerResponse(containerResponse);
            
            var container = containerResponse.Container;
            return container;
        }

        private async Task<Database> CreateDatabaseIfNotExistsAsync(string databaseId, CancellationToken cancellationToken = default)
        {
            var databaseResponse = await _client.CreateDatabaseIfNotExistsAsync(databaseId, cancellationToken: cancellationToken);
            _logger.LogDatabaseResponse(databaseResponse);
             
            var database = databaseResponse.Database;
            return database;
        }
    }
}