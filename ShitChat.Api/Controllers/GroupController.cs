using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ShitChat.Api.Hubs;
using ShitChat.Application.DTOs;
using ShitChat.Application.Interfaces;
using ShitChat.Application.Requests;
using ShitChat.Application.Services;
using ShitChat.Domain.Entities;

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
        response.Message = "SuccessCreatedGroup";

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
    public async Task<ActionResult<GenericResponse<IEnumerable<GroupMemberDto>>>> GetGroupMembers(Guid groupGuid)
    {
        var response = new GenericResponse<IEnumerable<GroupMemberDto>>();

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
    /// List group messages v2
    /// </summary>
    [Authorize(Policy = "GroupMember")]
    [HttpGet("{groupGuid}/messages")]
    public async Task<ActionResult<GenericResponse<IEnumerable<MessageDto>>>> GetGroupMessages(Guid groupGuid, [FromQuery] Guid? lastMessageId, [FromQuery] int take = 40)
    {
        var response = new GenericResponse<IEnumerable<MessageDto>>();

        var (success, message, messages) = await _groupService.GetGroupMessagesAsync(groupGuid, lastMessageId, take);

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

        await _hubContext.Clients.Group(groupGuid.ToString()).SendAsync("ReceiveMessage", messages, groupGuid);

        response.Message = message;
        response.Data = messages;

        return Ok(response);
    }

    /// <summary>
    /// Get group roles
    /// </summary>
    [Authorize(Policy = "GroupMember")]
    [HttpGet("{groupGuid}/roles")]
    public async Task<ActionResult<GenericResponse<IEnumerable<GroupRoleDto>>>> GetGroupRoles(Guid groupGuid)
    {
        var response = new GenericResponse<IEnumerable<GroupRoleDto>>();

        var (success, message, roles) = await _groupService.GetGroupRolesAsync(groupGuid);

        if (!success)
        {
            response.Errors.Add("Error", new List<string> { message });
            return BadRequest(response);
        }

        response.Message = message;
        response.Data = roles;

        return Ok(response);
    }

    /// <summary>
    /// Add role to user in group
    /// </summary>
    [Authorize(Policy = "GroupMember")]
    [HttpPost("{groupGuid}/user/{userId}/roles")] // Should change this & use Admin policy
    public async Task<ActionResult<GenericResponse<object>>> AddRoleToUser(Guid groupGuid, string userId, [FromBody] AddRoleToUserRequest request)
    {
        var response = new GenericResponse<object>();
        var (success, message, dto) = await _groupService.AddRoleToUser(groupGuid, userId, request.RoleId);

        if (!success)
        {
            response.Errors.Add("Error", new List<string> { message });
            return BadRequest(response);
        }

        await _hubContext.Clients.Group(groupGuid.ToString()).SendAsync("UserAddedRole", dto.GroupId, dto.UserId, dto.RoleId);

        response.Message = message;
        response.Data = dto;

        return Ok(response);
    }

    /// <summary>
    /// Remove role from user in group
    /// </summary>
    [Authorize(Policy = "GroupMember")]
    [HttpDelete("{groupGuid}/user/{userId}/roles")] // Should change this & use Admin policy
    public async Task<ActionResult<GenericResponse<object>>> RemoveRoleFromUser(Guid groupGuid, string userId, [FromBody] RemoveRoleFromUserRequest request)
    {
        var response = new GenericResponse<object>();
        var (success, message, dto) = await _groupService.RemoveRoleFromUser(groupGuid, userId, request.RoleId);

        if (!success)
        {
            response.Errors.Add("Error", new List<string> { message });
            return BadRequest(response);
        }

        await _hubContext.Clients.Group(groupGuid.ToString()).SendAsync("UserRemovedRole", dto.GroupId, dto.UserId, dto.RoleId);

        response.Message = message;
        response.Data = dto;

        return Ok(response);
    }

    /// <summary>
    /// Create group role
    /// </summary>
    [HttpPost("{groupGuid}/roles")]
    public async Task<ActionResult<GenericResponse<GroupRoleDto>>> CreateRole(Guid groupGuid, [FromBody] CreateGroupRoleRequest request)
    {
        var validator = _serviceProvider.GetRequiredService<IValidator<CreateGroupRoleRequest>>();
        var validationResult = await validator.ValidateAsync(request);
        var response = new GenericResponse<GroupRoleDto?>();

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

        var (success, message, dto) = await _groupService.CreateRoleAsync(groupGuid, request);

        if (!success)
        {
            response.Message = message;
            response.Data = null;
            return BadRequest(response);
        }

        await _hubContext.Clients.Group(groupGuid.ToString()).SendAsync("RoleCreated", groupGuid, dto);

        response.Data = dto;
        response.Message = "SuccessCreatedGroupRole";

        return Ok(response);
    }


    /// <summary>
    /// Edit group role
    /// </summary>
    [HttpPut("{groupGuid}/roles/{roleId}")]
    public async Task<ActionResult<GenericResponse<GroupRoleDto>>> EditRole(Guid groupGuid, Guid roleId, [FromBody] EditGroupRoleRequest request)
    {
        var validator = _serviceProvider.GetRequiredService<IValidator<EditGroupRoleRequest>>();
        var validationResult = await validator.ValidateAsync(request);
        var response = new GenericResponse<GroupRoleDto?>();

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

        var (success, message, dto) = await _groupService.EditRoleAsync(roleId, request);

        if (!success)
        {
            response.Message = message;
            response.Data = null;
            return BadRequest(response);
        }

        await _hubContext.Clients.Group(groupGuid.ToString()).SendAsync("RoleEdited", groupGuid, dto);

        response.Data = dto;
        response.Message = "SuccessEditedGroupRole";

        return Ok(response);
    }
}
