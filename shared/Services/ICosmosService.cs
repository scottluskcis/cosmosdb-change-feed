using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Shared.Entities;

namespace Shared.Services
{
    public interface IReadOnlyCosmosService
    {
        Task<TEntity> ReadItemAsync<TEntity>(
            string id, 
            string partitionKey, 
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity;

        Task<IEnumerable<TEntity>> ReadItemsAsync<TEntity>(
            Expression<Func<TEntity, bool>> predicate = null,
            string partitionKey = "", 
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity;

        Task<IEnumerable<TEntity>> QueryItemsAsync<TEntity>(
            string sql, 
            string partitionKey = "",
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity;
    }

    public interface ICosmosService : IReadOnlyCosmosService
    {
        Task<Container> CreateContainerIfNotExistsAsync<TEntity>(
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity;

        Task<TEntity> CreateItemAsync<TEntity>(
            TEntity item,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity;

        Task<TEntity> ReplaceItemAsync<TEntity>(
            TEntity item, 
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity;

        Task<TEntity> UpsertItemAsync<TEntity>(
            TEntity item,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity;

        Task<TEntity> DeleteItemAsync<TEntity>(
            string id, 
            string partitionKey, 
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity;
    }

    public interface IBulkExecutorCosmosService : IReadOnlyCosmosService
    {
        Task<IEnumerable<TEntity>> BulkCreateItemsAsync<TEntity>(
            IList<TEntity> entities)
            where TEntity : BaseEntity;

        Task<IEnumerable<TEntity>> BulkReplaceItemsAsync<TEntity>(
            IList<TEntity> entities)
            where TEntity : BaseEntity;

        Task<IEnumerable<TEntity>> BulkUpsertItemsAsync<TEntity>(
            IList<TEntity> entities)
            where TEntity : BaseEntity;
    }
}