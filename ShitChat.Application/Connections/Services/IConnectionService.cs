namespace ShitChat.Application.Connections.Services;

public interface IConnectionService
{
    Task<(bool, string)> CreateConnectionAsync(string friendId);

    Task<(bool, string)> AcceptConnectionAsync(string friendId);

    Task<(bool, string)> DeleteConnectionAsync(string friendId);
}
