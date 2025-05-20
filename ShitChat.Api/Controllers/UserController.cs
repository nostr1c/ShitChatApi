using ShitChat.Domain.Entities;
using ShitChat.Application.DTOs;
using ShitChat.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShitChat.Api.Controllers;

[Authorize(AuthenticationSchemes = "Bearer")]
[ApiController]
[Route("api/v1/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IGroupService _groupService;
    private readonly string _imageStoragePath = "/Uploads";

    public UserController
    (
        IUserService userService,
        IGroupService groupService
    )
    {
        _userService = userService;
        _groupService = groupService;
    }

    /// <summary>
    /// Get specific user by Guid
    /// </summary>
    [HttpGet("{guid}")]
    public async Task<ActionResult<GenericResponse<UserDto>>> GetUserByGuid(string guid)
    {
        var response = new GenericResponse<UserDto>();

        var (success, user) = await _userService.GetUserByGuidAsync(guid);

        if (!success || user == null)
        {
            response.Errors.Add("Error", new List<string> { "ErrorUserNotFound" });
            return NotFound(response);
        }

        var dto = new UserDto
        {
            Id = user.Id,
            Username = user.UserName,
            Email = user.Email,
            Avatar = user.AvatarUri,
            CreatedAt = user.CreatedAt
        };
       
        response.Data = dto;

        return Ok(response);
    }

    /// <summary>
    /// Update avatar
    /// </summary>
    [HttpPut("Avatar")]
    public async Task<ActionResult<GenericResponse<string?>>> UpdateAvatar(IFormFile avatar)
    {
        var response = new GenericResponse<string?>();

        var (success, message, imageName) = await _userService.UpdateAvatarAsync(avatar); 

        if (!success || message == null)
        {
            response.Message = message;
            return BadRequest(response);
        }

        response.Data = imageName;
        response.Message = message;
        
        return Ok(response);
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
        var response = new GenericResponse<List<ConnectionDto>>();
        // TODO: Change to groupservice
        var connections = await _userService.GetConnectionsAsync();

        response.Data = connections;

        return Ok(response);
    }

    /// <summary>
    /// Get user groups
    /// </summary>
    [HttpGet("Groups")]
    public async Task<ActionResult<GenericResponse<List<GroupDto>>>> GetGroups()
    {
        var response = new GenericResponse<List<GroupDto>>();
        var groups = await _groupService.GetUserGroupsAsync();

        response.Data = groups;

        return Ok(response);
    }
}
