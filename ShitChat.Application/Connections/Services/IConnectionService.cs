using ShitChat.Application.Connections.DTOs;
using ShitChat.Shared.Enums;

namespace ShitChat.Application.Connections.Services;

public interface IConnectionService
{
    Task<(bool, ConnectionActionResult, ConnectionActionDto?)> CreateConnectionAsync(string friendId);

    Task<(bool, ConnectionActionResult, ConnectionActionDto?)> AcceptConnectionAsync(string friendId);

    Task<(bool, ConnectionActionResult, ConnectionActionDto?)> DeleteConnectionAsync(string friendId);
}
