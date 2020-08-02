using System.Threading;
using System.Threading.Tasks;
using Shared.Entities;

namespace Shared.Services
{
    public interface ICosmosService<TEntity>
        where TEntity : BaseEntity
    {
        Task<TEntity> CreateItemAsync(
            TEntity item, 
            CancellationToken cancellationToken = default);

        Task<TEntity> ReadItemAsync(
            string id, 
            string partitionKey, 
            CancellationToken cancellationToken = default);

        Task<TEntity> ReplaceItemAsync(
            TEntity item, 
            CancellationToken cancellationToken = default);

        Task<TEntity> DeleteItemAsync(
            string id, 
            string partitionKey, 
            CancellationToken cancellationToken = default);
    }
}