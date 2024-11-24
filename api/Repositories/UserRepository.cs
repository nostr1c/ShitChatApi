using Dapper;
using System.Data;
using api.Models;
using api.Repositories.Exceptions;

namespace api.Repositories
{
    public class UserRepository
    {
        private readonly IDbConnection _connection;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(IDbConnection connection, ILogger<UserRepository> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            try
            {
                string query = "SELECT * FROM Users";
                return await _connection.QueryAsync<User>(query);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error fetching users.", ex);
            }
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            try
            {
                string query = "SELECT * FROM Users WHERE UserId = @UserId";
                return await _connection.QuerySingleOrDefaultAsync<User>(query, new { UserId = userId });
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Error fetching user with ID: {userId}", ex);
            }
        }

        public async Task<bool> UsernameAlreadyExists(string username)
        {
            try
            {
                string query = @"SELECT 1 UserId FROM Users WHERE Username = @Username";
                var result = await _connection.ExecuteScalarAsync<int?>(query, new { UserName = username });
                return result.HasValue;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error checking username", ex);
            }
        }
    }
}
