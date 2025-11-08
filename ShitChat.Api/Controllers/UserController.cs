using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShitChat.Application.Connections.DTOs;
using ShitChat.Application.DTOs;
using ShitChat.Application.Groups.DTOs;
using ShitChat.Application.Roles.DTOs;
using ShitChat.Application.Users.DTOs;
using ShitChat.Application.Users.Services;
using ShitChat.Domain.Entities;

namespace ShitChat.Api.Controllers;

[Authorize(AuthenticationSchemes = "Bearer")]
[ApiController]
[Route("api/v1/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly string _imageStoragePath = "/Uploads";
    private readonly ElasticsearchClient _elastic;

    public UserController
    (
        IUserService userService,
        ElasticsearchClient elastic
    )
    {
        _userService = userService;
        _elastic = elastic;
    }

    /// <summary>
    /// Get specific user by Guid
    /// </summary>
    [HttpGet("{guid}")]
    public async Task<ActionResult<GenericResponse<UserDto>>> GetUserByGuid(string guid)
    {
        var (success, message, userDto) = await _userService.GetUserByGuidAsync(guid);

        if (!success || userDto == null)
            return NotFound(ResponseHelper.Error<UserDto>(message: message, status: StatusCodes.Status404NotFound));

        return Ok(ResponseHelper.Success(message, userDto));
    }

    /// <summary>
    /// Update avatar
    /// </summary>
    [HttpPut("Avatar")]
    public async Task<ActionResult<GenericResponse<string?>>> UpdateAvatar(IFormFile avatar)
    {
        var (success, userServiceMessage, uploadServiceMessage, imageName) = await _userService.UpdateAvatarAsync(avatar);

        if (!success)
        {
            var errorMessage = uploadServiceMessage.ToString() ?? userServiceMessage.ToString() ?? "UnknownError";
            return BadRequest(ResponseHelper.Error<IEnumerable<MessageDto>>(errorMessage));
        }

        return Ok(ResponseHelper.Success(userServiceMessage, imageName));
    }

    /// <summary>
    /// Get user avatar
    /// </summary>
    [AllowAnonymous]
    [HttpGet("Avatar/{imageId}")]
    public IActionResult GetAvatar(string imageId)
    {
        var response = new GenericResponse<bool>();

        var filePath = Path.Combine(_imageStoragePath, imageId);
        if (!System.IO.File.Exists(filePath))
        {
            return NotFound("Image not found.");
        }

        var imageStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return File(imageStream, "image/jpeg");
    }

    /// <summary>
    /// Get user connections
    /// </summary>
    [HttpGet("Connections")]
    public async Task<ActionResult<GenericResponse<List<ConnectionDto>>>> GetConnections()
    {
        var (success, message, connections) = await _userService.GetConnectionsAsync();

        if (!success || connections == null)
            return BadRequest(ResponseHelper.Error<List<ConnectionDto>>(message));

        return Ok(ResponseHelper.Success(message, connections));
    }

    /// <summary>
    /// Get user groups
    /// </summary>
    [HttpGet("Groups")]
    public async Task<ActionResult<GenericResponse<List<GroupDto>>>> GetGroups()
    {
        var (success, message, groups) = await _userService.GetUserGroupsAsync();

        if (!success || groups == null)
            return BadRequest(ResponseHelper.Error<List<GroupDto>>(message));

        return Ok(ResponseHelper.Success(message, groups));
    }

    [AllowAnonymous]
    [HttpGet("Roles")]
    public async Task<ActionResult<GenericResponse<IEnumerable<UserWithRoles>>>> GetUsersWithRoles()
    {
        var (success, message, usersWithRoles) = await _userService.GetUsersWithRolesAsync();

        if (!success || usersWithRoles == null)
            return BadRequest(ResponseHelper.Error<IEnumerable<UserWithRoles>>(message));

        return Ok(ResponseHelper.Success(message, usersWithRoles));
    }

    [AllowAnonymous]
    [HttpPost("index-all")]
    public async Task<ActionResult<GenericResponse<string>>> IndexAllUsers()
    {
        var (success, message) = await _userService.IndexAllUsersAsync();

        if (!success)
            return BadRequest(ResponseHelper.Error<string>(message));

        return Ok(ResponseHelper.Success(message, message));
    }

    [AllowAnonymous]
    [HttpGet("search")]
    public async Task<IActionResult> SearchUsers([FromQuery] string query)
    {
        var (success, message, result) = await _userService.SearchUsersAsync(query);
        if (!success || result == null)
            return BadRequest(ResponseHelper.Error<IReadOnlyCollection<User>>(message));

        return Ok(ResponseHelper.Success(message, result));
    }
}
