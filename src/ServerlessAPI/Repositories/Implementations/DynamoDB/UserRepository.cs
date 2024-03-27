using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using ServerlessAPI.Entities.Implementations;
using ServerlessAPI.Entities.Interface;
using ServerlessAPI.Repositories.Interface;
using ServerlessAPI.Models.Interface;
namespace ServerlessAPI.Repositories.Implementations.DynamoDB
{

    public class UserRepositoryDynamoDB : IUserRepository
    {
        private readonly IDynamoDBContext context;
        private readonly ILogger<UserRepositoryDynamoDB> logger;

        public UserRepositoryDynamoDB(IDynamoDBContext context, ILogger<UserRepositoryDynamoDB> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<bool> CreateAsync(IUser user)
        {
            try
            {
                var userEntity = new UserEntity
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    LastName = user.LastName
                };
                userEntity.Id = Guid.NewGuid().ToString();
                Console.Write("adding user");
                await context.SaveAsync(userEntity, new DynamoDBOperationConfig { OverrideTableName = "USERS" });
                logger.LogInformation("user {} is added", userEntity.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "fail to persist to DynamoDb Table");
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteAsync(IUserEntity user)
        {
            bool result;
            try
            {
                // Delete the user.
                await context.DeleteAsync<IUserEntity>(user.Id, new DynamoDBOperationConfig { OverrideTableName = "USERS" });
                // Try to retrieve deleted user. It should return null.
                IUserEntity deleteduser = await context.LoadAsync<IUserEntity>(user.Id, new DynamoDBContextConfig
                {
                    ConsistentRead = true
                });

                result = deleteduser == null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "fail to delete user from DynamoDb Table");
                result = false;
            }

            if (result) logger.LogInformation("user {Id} is deleted", user);

            return result;
        }

        public async Task<bool> UpdateAsync(IUserEntity user)
        {
            if (user == null) return false;

            try
            {
                await context.SaveAsync(user, new DynamoDBOperationConfig { OverrideTableName = "USERS" });
                logger.LogInformation("user {Id} is updated", user);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "fail to update user from DynamoDb Table");
                return false;
            }

            return true;
        }

        public async Task<IUserEntity?> GetByIdAsync(string id)
        {
            try
            {
                return await context.LoadAsync<UserEntity>(id, new DynamoDBOperationConfig { OverrideTableName = "Users" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "fail to update user from DynamoDb Table");
                return null;
            }
        }

        public async Task<IList<IUserEntity>> GetAllAsync(int limit = 10)
        {
            var result = new List<UserEntity>();

            try
            {
                if (limit <= 0)
                {
                    return result.Cast<IUserEntity>().ToList();
                }

                var filter = new ScanFilter();
                filter.AddCondition("Id", ScanOperator.IsNotNull);
                var scanConfig = new ScanOperationConfig()
                {
                    Limit = limit,
                    Filter = filter
                };
                var queryResult = context.FromScanAsync<UserEntity>(scanConfig, new DynamoDBOperationConfig { OverrideTableName = "USERS" });

                do
                {
                    result.AddRange(await queryResult.GetNextSetAsync());
                }
                while (!queryResult.IsDone && result.Count < limit);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "fail to list users from DynamoDb Table");
                return new List<IUserEntity>();
            }

            return result.Cast<IUserEntity>().ToList();
        }
    }
}
