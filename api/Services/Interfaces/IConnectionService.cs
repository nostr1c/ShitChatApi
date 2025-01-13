namespace api.Services.Interfaces;

public interface IConnectionService
{
    Task<(bool Success, string Message)> CreateConnectionAsync(string userName, string friendId);

    Task<(bool Success, string Message)> AcceptConnectionAsync(string userName, string friendId);

    Task<(bool Success, string Message)> DeleteConnectionAsync(string userName, string friendId);
}
