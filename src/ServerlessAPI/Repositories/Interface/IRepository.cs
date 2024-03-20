using Amazon.DynamoDBv2.Model;
using ServerlessAPI.Entities.Interface;
using ServerlessAPI.Models.Interface;
using System.Collections.Generic;
namespace ServerlessAPI.Repositories.Interface
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<bool> CreateAsync(TEntity entity);
        Task<bool> DeleteAsync(TEntity entity);
        Task<IList<TEntity>> GetAllAsync(int limit = 10);

        Task<TEntity?> GetByIdAsync(string id);
          
        Task<bool> UpdateAsync(TEntity entity);
    }
}