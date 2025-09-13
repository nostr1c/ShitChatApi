using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShitChat.Application.DTOs;
using ShitChat.Application.Interfaces;

namespace ShitChat.Api.Controllers;

[Authorize(AuthenticationSchemes = "Bearer")]
[ApiController]
[Route("api/v1/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly string _imageStoragePath = "/Uploads";

    public UserController
    (
        IUserService userService
    )
    {
        _userService = userService;
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

        return Ok(new GenericResponse<UserDto>{
            Data = userDto,
            Message = message,
            Status = StatusCodes.Status200OK
        });
    }

    /// <summary>
    /// Update avatar
    /// </summary>
    [HttpPut("Avatar")]
    public async Task<ActionResult<GenericResponse<string?>>> UpdateAvatar(IFormFile avatar)
    {
        var (success, message, imageName) = await _userService.UpdateAvatarAsync(avatar);

        if (!success || message == null)
            return BadRequest(ResponseHelper.Error<string?>(message));

        return Ok(new GenericResponse<string?>
        {
            Data = imageName,
            Message = message,
            Status = StatusCodes.Status200OK
        });
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

        return Ok(new GenericResponse<List<ConnectionDto>>
        {
            Data = connections,
            Message = message,
            Status = StatusCodes.Status200OK
        });
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

        return Ok(new GenericResponse<List<GroupDto>>
        {
            Data = groups,
            Message = message,
            Status = StatusCodes.Status200OK
        });
    }
}
