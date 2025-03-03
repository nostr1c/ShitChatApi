using ShitChat.Application.DTOs;
using ShitChat.Application.Requests;
using ShitChat.Application.Interfaces;
using ShitChat.Api.Hubs;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ShitChat.Api.Controllers;

[Authorize(AuthenticationSchemes = "Bearer")]
[ApiController]
[Route("api/v1/[controller]")]
public class GroupController : ControllerBase
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IGroupService _groupService;
    private readonly ILogger<GroupController> _logger;
    private readonly IHubContext<ChatHub> _hubContext;

    public GroupController
    (
        IServiceProvider serviceProvider,
        IGroupService groupService,
        ILogger<GroupController> logger,
        IHubContext<ChatHub> hubContext
    )
    {
        _serviceProvider = serviceProvider;
        _groupService = groupService;
        _logger = logger;
        _hubContext = hubContext;
    }

    /// <summary>
    /// Create group
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<GenericResponse<GroupDto>>> CreateGroup([FromBody] CreateGroupRequest request)
    {
        var validator = _serviceProvider.GetRequiredService<IValidator<CreateGroupRequest>>();
        var validationResult = await validator.ValidateAsync(request);
        var response = new GenericResponse<GroupDto>();

        if (!validationResult.IsValid)
        {
            response.Errors = validationResult.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    x => x.Key,
                    x => x.Select(y => y.ErrorMessage).ToList()
                );

            response.Message = "ErrorValidationFailed";
            return BadRequest(response);
        }

        var group = await _groupService.CreateGroupAsync(request);

        response.Data = group;

        return Ok(response);
    }

    /// <summary>
    /// Get specific group
    /// </summary>
    [Authorize(Policy = "GroupMember")]
    [HttpGet("{groupGuid}")]
    public async Task<ActionResult<GenericResponse<GroupDto>>> GetGroupByGuid(Guid groupGuid)
    {
        var response = new GenericResponse<GroupDto>();

        var group = await _groupService.GetGroupByGuidAsync(groupGuid);

        if (group == null)
        {
            response.Message = "ErrorGroupNotFound";
            return NotFound(response);
        }

        response.Data = group;

        return Ok(response);
    }

    /// <summary>
    /// Add member to group
    /// </summary>
    [HttpPost("{groupGuid}/members")]
    public async Task<ActionResult<GenericResponse<UserDto>>> AddUserToGroup(Guid groupGuid, [FromBody] string userId)
    {
        var response = new GenericResponse<UserDto>();

        var (success, message, userDto) = await _groupService.AddUserToGroupAsync(groupGuid, userId);

        if (!success || userDto == null)
        {
            response.Errors.Add("Error", new List<string> { message });
            return BadRequest(response);
        }

        response.Message = message;
        response.Data = userDto;

        return Ok(response);
    }

    /// <summary>
    /// List group members
    /// </summary>
    [Authorize(Policy = "GroupMember")]
    [HttpGet("{groupGuid}/members")]
    public async Task<ActionResult<GenericResponse<IEnumerable<UserDto>>>> GetGroupMembers(Guid groupGuid)
    {
        var response = new GenericResponse<IEnumerable<UserDto>>();

        var (success, message, users) = await _groupService.GetGroupMembersAsync(groupGuid);

        if (!success)
        {
            response.Errors.Add("Error", new List<string> { message });
            return BadRequest(response);
        }

        response.Message = message;
        response.Data = users;

        return Ok(response);
    }

    /// <summary>
    /// List group messages
    /// </summary>
    [Authorize(Policy = "GroupMember")]
    [HttpGet("{groupGuid}/messages")]
    public async Task<ActionResult<GenericResponse<IEnumerable<MessageDto>>>> GetGroupMessages(Guid groupGuid)
    {
        var response = new GenericResponse<IEnumerable<MessageDto>>();

        var (success, message, messages) = await _groupService.GetGroupMessagesAsync(groupGuid);

        if (!success)
        {
            response.Errors.Add("Error", new List<string> { message });
            return BadRequest(response);
        }

        response.Message = message;
        response.Data = messages;

        return Ok(response);
    }
    /// <summary>
    /// Send group message
    /// </summary>
    [Authorize(Policy = "GroupMember")]
    [HttpPost("{groupGuid}/messages")]
    public async Task<ActionResult<GenericResponse<IEnumerable<MessageDto>>>> SendMessage(Guid groupGuid, [FromBody] SendMessageRequest request)
    {
        var validator = _serviceProvider.GetRequiredService<IValidator<SendMessageRequest>>();
        var validationResult = await validator.ValidateAsync(request);
        var response = new GenericResponse<MessageDto>();

        if (!validationResult.IsValid)
        {
            response.Errors = validationResult.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    x => x.Key,
                    x => x.Select(y => y.ErrorMessage).ToList()
                );

            response.Message = "ErrorValidationFailed";
            return BadRequest(response);
        }

        var (success, message, messages) = await _groupService.SendMessageAsync(groupGuid, request);

        if (!success)
        {
            response.Errors.Add("Error", new List<string> { message });
            return BadRequest(response);
        }

        await _hubContext.Clients.Group(groupGuid.ToString()).SendAsync("ReceiveMessage", message);

        response.Message = message;
        response.Data = messages;

        return Ok(response);
    }
}
