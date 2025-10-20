using ShitChat.Shared.Enums;

namespace ShitChat.Application.Connections.Services;

public interface IConnectionService
{
    Task<(bool, ConnectionActionResult)> CreateConnectionAsync(string friendId);

    Task<(bool, ConnectionActionResult)> AcceptConnectionAsync(string friendId);

    Task<(bool, ConnectionActionResult)> DeleteConnectionAsync(string friendId);
}
