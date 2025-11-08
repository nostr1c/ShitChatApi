using ShitChat.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShitChat.Application.Connections.Services;
using ShitChat.Application.Groups.Services;
using Microsoft.AspNetCore.SignalR;
using ShitChat.Api.Hubs;
using ShitChat.Application.Connections.DTOs;

namespace ShitChat.Api.Controllers;

[Authorize(AuthenticationSchemes = "Bearer")]
[ApiController]
[Route("api/v1/[controller]")]
public class ConnectionController : ControllerBase
{
    private readonly ILogger<ConnectionController> _logger;
    private readonly IConnectionService _connectionService;
    private readonly IPresenceService _presenceService;
    private readonly IHubContext<ChatHub> _hubContext;
 
    public ConnectionController
    (
        ILogger<ConnectionController> logger,
        IConnectionService connectionService,
        IPresenceService presenceService,
        IHubContext<ChatHub> hubContext
    )
    {
        _logger = logger;
        _connectionService = connectionService;
        _presenceService = presenceService;
        _hubContext = hubContext;
    }

    /// <summary>
    /// Create a new connection request
    /// </summary>
    [HttpPost("Add")]
    public async Task<ActionResult<GenericResponse<ConnectionDto?>>> CreateConnection([FromBody] string friendId)
    {
        var (success, message, connectionDto) = await _connectionService.CreateConnectionAsync(friendId);

        if (!success || connectionDto == null)
            return BadRequest(ResponseHelper.Error<ConnectionDto?>(message));

        var connections = await _presenceService.GetUserConnections(friendId);

        if (connections != null && connections.Length > 0)
        {
            await _hubContext.Clients.Clients(connections).SendAsync("ReceiveFriendRequest", connectionDto.ToReceiver);
        }

        return Ok(ResponseHelper.Success(message, connectionDto.ToRequesterer, StatusCodes.Status201Created));
    }

    /// <summary>
    /// Accept a friend request.
    /// </summary>
    [HttpPut("Accept")]
    public async Task<ActionResult<GenericResponse<object>>> AcceptConnection([FromQuery] string friendId)
    {
        var (success, message, connectionDto) = await _connectionService.AcceptConnectionAsync(friendId);

        if (!success || connectionDto == null)
            return BadRequest(ResponseHelper.Error<object>(message));

        var connections = await _presenceService.GetUserConnections(friendId);

        if (connections != null && connections.Length > 0)
        {
            await _hubContext.Clients.Clients(connections).SendAsync("ReceiveFriendRequestAccepted", connectionDto.ToReceiver);
        }

        return Ok(ResponseHelper.Success(message, connectionDto.ToRequesterer));
    }


    /// <summary>
    /// Delete a friend
    /// </summary>
    [HttpDelete("Delete")]
    public async Task<ActionResult<GenericResponse<object>>> DeleteConnection([FromQuery] string friendId)
    {
        var (success, message, connectionDto) = await _connectionService.DeleteConnectionAsync(friendId);

        if (!success || connectionDto == null)
            return BadRequest(ResponseHelper.Error<object>(message));

        var connections = await _presenceService.GetUserConnections(friendId);

        if (connections != null && connections.Length > 0)
        {
            await _hubContext.Clients.Clients(connections).SendAsync("ReceiveFriendRequestDeleted", connectionDto.ToReceiver);
        }

        return Ok(ResponseHelper.Success(message, connectionDto.ToRequesterer));
    }
}
