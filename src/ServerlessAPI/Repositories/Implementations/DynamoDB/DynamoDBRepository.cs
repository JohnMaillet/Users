using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.Extensions.Logging;
using ServerlessAPI.Entities.Interface;
using ServerlessAPI.Models.Interface;
using ServerlessAPI.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerlessAPI.Repositories.Implementations.DynamoDB
{
    public class DynamoDBRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {
        private readonly IDynamoDBContext _context;
        private readonly ILogger<DynamoDBRepository<TEntity>> _logger;

        public DynamoDBRepository(IDynamoDBContext context, ILogger<DynamoDBRepository<TEntity>> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> CreateAsync(TEntity entity)
        {
            try
            {
                await _context.SaveAsync(entity, new DynamoDBOperationConfig { OverrideTableName = typeof(TEntity).Name });
                _logger.LogInformation("Entity {Id} is added", entity.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fail to persist entity to DynamoDB Table");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(TEntity entity)
        {
            try
            {
                await _context.DeleteAsync<TEntity>(entity.Id, new DynamoDBOperationConfig { OverrideTableName = typeof(TEntity).Name });
                _logger.LogInformation("Entity {Id} is deleted", entity.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fail to delete entity from DynamoDB Table");
                return false;
            }
        }

        public async Task<bool> UpdateAsync(TEntity entity)
        {
            try
            {
                await _context.SaveAsync(entity, new DynamoDBOperationConfig { OverrideTableName = typeof(TEntity).Name });
                _logger.LogInformation("Entity {Id} is updated", entity.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fail to update entity in DynamoDB Table");
                return false;
            }
        }

        public async Task<TEntity?> GetByIdAsync(string id)
        {
            try
            {
                return await _context.LoadAsync<TEntity>(id, new DynamoDBOperationConfig { OverrideTableName = typeof(TEntity).Name });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fail to retrieve entity from DynamoDB Table");
                return null;
            }
        }

        public async Task<IList<TEntity>> GetAllAsync(int limit = 10)
        {
            try
            {
                if (limit <= 0)
                    return new List<TEntity>();

                var filter = new ScanFilter();
                filter.AddCondition("Id", ScanOperator.IsNotNull);
                var scanConfig = new ScanOperationConfig()
                {
                    Limit = limit,
                    Filter = filter
                };
                var queryResult = _context.FromScanAsync<TEntity>(scanConfig, new DynamoDBOperationConfig { OverrideTableName = typeof(TEntity).Name });

                var result = new List<TEntity>();
                do
                {
                    result.AddRange(await queryResult.GetNextSetAsync());
                } while (!queryResult.IsDone && result.Count < limit);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fail to list entities from DynamoDB Table");
                return new List<TEntity>();
            }
        }
    }
}

