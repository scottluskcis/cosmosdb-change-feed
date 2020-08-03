using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Shared.Configuration;
using Microsoft.Extensions.Logging;
using Shared.Entities;
using Shared.Extensions;
using Shared.Helpers;

namespace Shared.Services
{
    public class CosmosService : ICosmosService, IBulkExecutorCosmosService
    {
        private readonly CosmosClient _client;
        private readonly CosmosDbConfiguration _config;
        private readonly ILogger _logger;

        public CosmosService(CosmosClient client, CosmosDbConfiguration configuration, ILogger<CosmosService> logger)
        {
            _client = client;
            _config = configuration;
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

            var itemResponse = await container.ReplaceItemAsync(item, item.GetId(), pk, cancellationToken: cancellationToken);
            _logger.LogItemResponse(itemResponse);
            
            var result = itemResponse.Resource;
            return result;
        }
        
        public async Task<TEntity> UpsertItemAsync<TEntity>(
            TEntity item, 
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity
        {
            var container = GetContainer<TEntity>();

            var pk = item.GetPartitionKey();
            _logger.LogPartitionKey(pk, container);

            var itemResponse = await container.UpsertItemAsync(item, pk, cancellationToken: cancellationToken);
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
        
        public async Task<IEnumerable<TEntity>> BulkCreateItemsAsync<TEntity>(IList<TEntity> entities)
            where TEntity : BaseEntity
        {
            var container = GetContainer<TEntity>();

            var result = await BulkOperationAsync(entities, item =>
            {
                var partitionKey = item.GetPartitionKey();
                return container.CreateItemAsync(item, partitionKey);
            });

            return result;
        }

        public async Task<IEnumerable<TEntity>> BulkUpsertItemsAsync<TEntity>(IList<TEntity> entities)
            where TEntity : BaseEntity
        {
            var container = GetContainer<TEntity>();

            var result = await BulkOperationAsync(entities, item =>
            {
                var partitionKey = item.GetPartitionKey();
                return container.UpsertItemAsync(item, partitionKey);
            });

            return result;
        }

        public async Task<IEnumerable<TEntity>> BulkReplaceItemsAsync<TEntity>(IList<TEntity> entities)
            where TEntity : BaseEntity
        {
            var container = GetContainer<TEntity>();

            var result = await BulkOperationAsync(entities, item =>
            {
                var partitionKey = item.GetPartitionKey();
                return container.ReplaceItemAsync(item, item.GetId(), partitionKey);
            });

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
        
        private async Task<IEnumerable<TEntity>> BulkOperationAsync<TEntity>(IList<TEntity> entities, Func<TEntity, Task<ItemResponse<TEntity>>> operationFunc)
            where TEntity : BaseEntity
        {
            if (!(_config.AllowBulkExecution ?? false))
                throw new InvalidOperationException($"{nameof(_config.AllowBulkExecution)} must be true in order to perform this operation");
            
            var bulkOperations = new BulkOperations<TEntity>(entities.Count);
            foreach (var entity in entities)
                bulkOperations.Tasks.Add(operationFunc(entity).CaptureOperationResponse(entity));

            var bulkOperationResponse =  await bulkOperations.ExecuteAsync();
            _logger.LogBulkOperationResponse(bulkOperationResponse);

            if(bulkOperationResponse.Failures.Any())
                _logger.LogWarning("{FailureCount} items failed during bulk operation", bulkOperationResponse.Failures.Count);
            
            var result = bulkOperationResponse.Successes;
            return result;
        }

        private Database GetDatabase()
        {
            var database = _client.GetDatabase(_config.DatabaseId);
            return database;
        }

        public async Task<Container> CreateContainerIfNotExistsAsync<TEntity>(CancellationToken cancellationToken = default)
            where TEntity : BaseEntity
        {
            _logger.LogInformation("{Action} - Start", nameof(CreateContainerIfNotExistsAsync));

            var database = await CreateDatabaseIfNotExistsAsync(_config.DatabaseId, _config.Throughput, cancellationToken); 
            var props = typeof(TEntity).GetContainerProperties();

            var containerResponse = await database.CreateContainerIfNotExistsAsync(props, cancellationToken: cancellationToken);
            _logger.LogContainerResponse(containerResponse);
            
            var container = containerResponse.Container;
            
            _logger.LogInformation("{Action} - Start", nameof(CreateContainerIfNotExistsAsync));

            return container;
        }

        private async Task<Database> CreateDatabaseIfNotExistsAsync(string databaseId, int? throughput = null, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("{Action} - Start", nameof(CreateDatabaseIfNotExistsAsync));

            var databaseResponse = await _client.CreateDatabaseIfNotExistsAsync(databaseId, throughput, cancellationToken: cancellationToken);
            _logger.LogDatabaseResponse(databaseResponse);
             
            var database = databaseResponse.Database;
            
            _logger.LogInformation("{Action} - End", nameof(CreateDatabaseIfNotExistsAsync));

            return database;
        }
    }
}