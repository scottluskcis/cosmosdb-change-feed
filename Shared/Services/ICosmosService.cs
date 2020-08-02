using System.Threading.Tasks;
using Shared.Entities;

namespace Shared.Services
{
    public interface ICosmosService<TEntity>
        where TEntity : BaseEntity
    {
        Task<TEntity> CreateItemAsync(TEntity item);

        Task<TEntity> ReadItemAsync(string id, string partitionKeyValue);

        Task<TEntity> ReplaceItemAsync(TEntity item);

        Task<TEntity> DeleteItemAsync(string id, string partitionKeyValue);
    }
}