using ShitChat.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShitChat.Application.Connections.Services;

namespace ShitChat.Api.Controllers;

[Authorize(AuthenticationSchemes = "Bearer")]
[ApiController]
[Route("api/v1/[controller]")]
public class ConnectionController : ControllerBase
{
    private readonly ILogger<ConnectionController> _logger;
    private readonly IConnectionService _connectionService;

    public ConnectionController
    (
        ILogger<ConnectionController> logger,
        IConnectionService connectionService
    )
    {
        _logger = logger;
        _connectionService = connectionService;
    }

    /// <summary>
    /// Create a new connection request
    /// </summary>
    [HttpPost("Add")]
    public async Task<ActionResult<GenericResponse<object>>> CreateConnection([FromBody] string friendId)
    {
        var (success, message) = await _connectionService.CreateConnectionAsync(friendId);

        if (!success)
            return BadRequest(ResponseHelper.Error<object>(message));

        return Ok(new GenericResponse<object>
        {
            Data = null,
            Message = message,
            Status = 200
        });
    }

    /// <summary>
    /// Accept a friend request.
    /// </summary>
    [HttpPut("Accept")]
    public async Task<ActionResult<GenericResponse<object>>> AcceptConnection([FromBody] string friendId)
    {
        var (success, message) = await _connectionService.AcceptConnectionAsync(friendId);

        if (!success)
            return BadRequest(ResponseHelper.Error<object>(message));

        return Ok(new GenericResponse<object>
        {
            Data = null,
            Message = message,
            Status = 200
        });
    }


    /// <summary>
    /// Delete a friend
    /// </summary>
    [HttpDelete("Delete")]
    public async Task<ActionResult<GenericResponse<object>>> DeleteConnection([FromBody] string friendId)
    {
        var (success, message) = await _connectionService.DeleteConnectionAsync(friendId);

        if (!success)
            return BadRequest(ResponseHelper.Error<object>(message));

        return Ok(new GenericResponse<object>
        {
            Data = null,
            Message = message,
            Status = 200
        });
    }
}
