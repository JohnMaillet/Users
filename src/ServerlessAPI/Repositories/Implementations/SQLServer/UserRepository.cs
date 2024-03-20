using Microsoft.Data.SqlClient;
using ServerlessAPI.Entities.Implementations;
using ServerlessAPI.Entities.Interface;
using ServerlessAPI.Repositories.Interface;
using ServerlessAPI.Models.Interface;
using ServerlessAPI.Utilities;

namespace ServerlessAPI.Repositories.Implementations.SQLServer
{
    public class UserRepositorySQLServer : IRepository
    {
        /*        private readonly string _connectionString =  "Data Source=localhost;" +
                    "Initial Catalog=ServiceData;" +
                    "Integrated Security=SSPI;" +
                    "TrustServerCertificate=True;";*/
        private readonly string _connectionString;
        private readonly ILogger<UserRepositorySQLServer> _logger;
        private readonly IErrorMessages _errorMessages;
        //string connectionString, 
        public UserRepositorySQLServer(string connectionString, ILogger<UserRepositorySQLServer> logger, IErrorMessages errorMessages)
        {
            _connectionString = connectionString;
            _logger = logger;
            _errorMessages = errorMessages;
        }

        public async Task<bool> CreateAsync(IUser user)
        {
            try
            {
                var userEntity = new UserEntity
                {
                    UserName = user.UserName,
                    PasswordHash = user.PasswordHash,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    LastName = user.LastName
                };
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string query = "INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, AccountCreationTimeStamp, LastModifiedBy) VALUES (@Username, @Email, @PasswordHash, @FirstName, @LastName, @AccountCreationTimeStamp, @LastModifiedBy)";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserName", userEntity.UserName);
                        command.Parameters.AddWithValue("@Email", userEntity.Email);
                        command.Parameters.AddWithValue("@PasswordHash", userEntity.PasswordHash);
                        command.Parameters.AddWithValue("@FirstName", userEntity.FirstName);
                        command.Parameters.AddWithValue("@LastName", userEntity.LastName);
                        command.Parameters.AddWithValue("@AccountCreationTimeStamp", DateTime.UtcNow);
                        command.Parameters.AddWithValue("@LastModifiedBy", userEntity.UserName);
                        // Add other parameters as needed...
                        await command.ExecuteNonQueryAsync();
                    }
                }

                _logger.LogInformation("User {Id} is added", userEntity.Id);
                return true;
            }
            catch (SqlException sqlEx)
            {
                if(sqlEx.Number == 2627)
                {
                    _logger.LogError(sqlEx, "Unique Constraint violation");
                    throw new Exception("Username or email already exists");
                } else
                {
                    _logger.LogError(sqlEx, "Unhandled SQL Error");
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to persist to SQL Server database");
                return false;
            }
        }


        public async Task<bool> DeleteAsync(IUserEntity user)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var sql = "DELETE FROM Users WHERE Id = @Id";
                    var command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@Id", user.Id);

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                _logger.LogError(ex, "Failed to delete user from SQL Server database");
                return false;
            }
        }

        public async Task<IUserEntity?> GetByIdAsync(string id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var sql = "SELECT * FROM Users WHERE Id = @Id";
                    var command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            // Map reader result to IUserEntity object
                            var user = new UserEntity(); // Create instance of UserEntity or use a mapper
                                                         // Map reader columns to user properties
                            user.Id = reader["Id"].ToString();
                            user.UserName = reader["UserName"].ToString();
                            user.Email = reader["Email"].ToString();
                            user.PasswordHash = reader["PasswordHash"].ToString();
                            user.FirstName = reader["FirstName"].ToString();
                            user.MiddleName = reader["MiddleName"]?.ToString();
                            user.LastName = reader["LastName"].ToString();

                            return user;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                _logger.LogError(ex, "Failed to get user by Id from SQL Server database");

            }
            return null;
        }

        public async Task<bool> UpdateAsync(IUserEntity user)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var sql = @"UPDATE Users
                        SET UserName = @UserName,
                            Email = @Email,
                            PasswordHash = @PasswordHash,
                            FirstName = @FirstName,
                            MiddleName = @MiddleName,
                            LastName = @LastName
                        WHERE Id = @Id";

                    var command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@Id", user.Id);
                    command.Parameters.AddWithValue("@UserName", user.UserName);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    command.Parameters.AddWithValue("@FirstName", user.FirstName);
                    command.Parameters.AddWithValue("@MiddleName", (object)user.MiddleName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@LastName", user.LastName);

                    int rowsAffected = await command.ExecuteNonQueryAsync();

                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                _logger.LogError(ex, "Failed to update user in SQL Server database");
                return false;
            }
        }

        public async Task<IList<IUserEntity>> GetUsersAsync(int limit = 10)
        {
            var result = new List<IUserEntity>();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var sql = @"SELECT TOP(@Limit) * 
                        FROM Users";

                    var command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@Limit", limit);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var userEntity = new UserEntity(); // Assuming UserEntity implements IUserEntity
                            userEntity.Id = reader["Id"].ToString();
                            userEntity.UserName = reader["UserName"].ToString();
                            userEntity.Email = reader["Email"].ToString();
                            userEntity.PasswordHash = reader["PasswordHash"].ToString();
                            userEntity.FirstName = reader["FirstName"].ToString();
                            userEntity.MiddleName = reader["MiddleName"] != DBNull.Value ? reader["MiddleName"].ToString() : null;
                            userEntity.LastName = reader["LastName"].ToString();
                            userEntity.AccountCreationTimeStamp = reader["AccountCreationTimeStamp"] != DBNull.Value ? reader["AccountCreationTimeStamp"].ToString() : null; ;
                            userEntity.LastLoginTime = reader["LastLoginTime"] != DBNull.Value ? reader["LastLoginTime"].ToString() : null; ;
                            userEntity.LastModifiedBy = reader["LastModifiedBy"] != DBNull.Value ? reader["LastModifiedBy"].ToString() : null; ;
                            result.Add(userEntity);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                _logger.LogError(ex, "Failed to get users from SQL Server database");
            }
            return result;
        }
    }
}
