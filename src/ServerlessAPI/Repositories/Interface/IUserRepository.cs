using Amazon.DynamoDBv2.Model;
using ServerlessAPI.Entities.Interface;
using ServerlessAPI.Models.Interface;
using System.Collections.Generic;
namespace ServerlessAPI.Repositories.Interface
{
    public interface IUserRepository
    {
        Task<bool> CreateAsync(IUser entity);
        Task<bool> DeleteAsync(IUserEntity entity);
        Task<IList<IUserEntity>> GetAllAsync(int limit = 10);

        Task<IUserEntity?> GetByIdAsync(string id);
          
        Task<bool> UpdateAsync(IUserEntity entity);
    }
}