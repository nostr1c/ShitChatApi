namespace ShitChat.Application.Interfaces;

public interface IConnectionService
{
    Task<(bool, string)> CreateConnectionAsync(string friendId);

    Task<(bool, string)> AcceptConnectionAsync(string friendId);

    Task<(bool, string)> DeleteConnectionAsync(string friendId);
}
