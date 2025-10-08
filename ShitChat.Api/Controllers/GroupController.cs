using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ShitChat.Api.Hubs;
using ShitChat.Application.DTOs;
using ShitChat.Application.Groups.DTOs;
using ShitChat.Application.Groups.Requests;
using ShitChat.Application.Groups.Services;
using ShitChat.Application.Users.DTOs;

namespace ShitChat.Api.Controllers;

[Authorize(AuthenticationSchemes = "Bearer")]
[ApiController]
[Route("api/v1/[controller]")]
public class GroupController : ControllerBase
{
    private readonly IGroupService _groupService;
    private readonly ILogger<GroupController> _logger;
    private readonly IHubContext<ChatHub> _hubContext;

    public GroupController
    (
        IGroupService groupService,
        ILogger<GroupController> logger,
        IHubContext<ChatHub> hubContext
    )
    {
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
        var (success, message, group) = await _groupService.CreateGroupAsync(request);
        if (!success || group == null)
            return BadRequest(ResponseHelper.Error<GroupDto>(message));

        return Ok(new GenericResponse<GroupDto>
        {
            Data = group,
            Message = message,
            Status = StatusCodes.Status201Created
        });
    }

    /// <summary>
    /// Get specific group
    /// </summary>
    [Authorize(Policy = "GroupMember")]
    [HttpGet("{groupGuid}")]
    public async Task<ActionResult<GenericResponse<GroupDto>>> GetGroupByGuid(Guid groupGuid)
    {
        var (success, message, group) = await _groupService.GetGroupByGuidAsync(groupGuid);
        if (!success || group == null)
            return BadRequest(ResponseHelper.Error<GroupDto>(message));

        return Ok(new GenericResponse<GroupDto>
        {
            Data = group,
            Message = message,
            Status = StatusCodes.Status200OK
        });
    }

    /// <summary>
    /// Edit specific group
    /// </summary>
    [Authorize(Policy = "CanManageServer")]
    [HttpPut("{groupGuid}")]
    public async Task<ActionResult<GenericResponse<GroupDto>>> EditGroup(Guid groupGuid, [FromBody] EditGroupRequest request)
    {
        var (success, message, group) = await _groupService.EditGroupAsync(groupGuid, request);
        if (!success || group == null)
            return BadRequest(ResponseHelper.Error<GroupDto>(message));

        await _hubContext.Clients.Group(groupGuid.ToString()).SendAsync("EditedGroup", groupGuid, group);

        return Ok(new GenericResponse<GroupDto>
        {
            Data = group,
            Message = message,
            Status = StatusCodes.Status200OK
        });
    }

    /// <summary>
    /// Delete specific group
    /// </summary>
    [Authorize(Policy = "CanManageServer")]
    [HttpDelete("{groupGuid}")]
    public async Task<ActionResult<GenericResponse<object>>> DeleteGroup(Guid groupGuid)
    {
        var (success, message) = await _groupService.DeleteGroupAsync(groupGuid);
        if (!success)
            return BadRequest(ResponseHelper.Error<object>(message));

        await _hubContext.Clients.Group(groupGuid.ToString()).SendAsync("DeletedGroup", groupGuid);

        return Ok(new GenericResponse<object>
        {
            Message = message,
            Status = StatusCodes.Status200OK
        });
    }

    /// <summary>
    /// Add member to group
    /// </summary>
    [Authorize(Policy = "GroupMember")]
    [HttpPost("{groupGuid}/members")]
    public async Task<ActionResult<GenericResponse<UserDto>>> AddUserToGroup(Guid groupGuid, [FromBody] string userId)
    {
        var (success, message, userDto) = await _groupService.AddUserToGroupAsync(groupGuid, userId);

        if (!success || userDto == null)
            return BadRequest(ResponseHelper.Error<UserDto>(message));

        return Ok(new GenericResponse<UserDto>
        {
            Data = userDto,
            Message = message,
            Status = StatusCodes.Status200OK
        });
    }

    /// <summary>
    /// kick member from group
    /// </summary>
    [Authorize(Policy = "CanKick")]
    [HttpPost("{groupGuid}/members/{userId}/kick")]
    public async Task<ActionResult<GenericResponse<object>>> KickUserFromGroup(Guid groupGuid, string userId)
    {
        var (success, message) = await _groupService.KickUserFromGroupAsync(groupGuid, userId);

        if (!success)
            return BadRequest(ResponseHelper.Error<object>(message));

        await _hubContext.Clients.Group(groupGuid.ToString()).SendAsync("RemoveMember", groupGuid, userId);

        return Ok(new GenericResponse<object>
        {
            Message = message,
            Status = StatusCodes.Status200OK
        });
    }

    /// <summary>
    /// List group members
    /// </summary>
    [Authorize(Policy = "GroupMember")]
    [HttpGet("{groupGuid}/members")]
    public async Task<ActionResult<GenericResponse<IEnumerable<GroupMemberDto>>>> GetGroupMembers(Guid groupGuid)
    {
        var (success, message, groupMemberDto) = await _groupService.GetGroupMembersAsync(groupGuid);

        if (!success)
            return BadRequest(ResponseHelper.Error<IEnumerable<GroupMemberDto>>(message));

        return Ok(new GenericResponse<IEnumerable<GroupMemberDto>>
        {
            Data = groupMemberDto,
            Message = message,
            Status = StatusCodes.Status200OK
        });
    }

    /// <summary>
    /// List group messages v2
    /// </summary>
    [Authorize(Policy = "GroupMember")]
    [HttpGet("{groupGuid}/messages")]
    public async Task<ActionResult<GenericResponse<IEnumerable<MessageDto>>>> GetGroupMessages(Guid groupGuid, [FromQuery] Guid? lastMessageId, [FromQuery] int take = 40)
    {
        var (success, message, messages) = await _groupService.GetGroupMessagesAsync(groupGuid, lastMessageId, take);

        if (!success)
            return BadRequest(ResponseHelper.Error<IEnumerable<MessageDto>>(message));

        return Ok(new GenericResponse<IEnumerable<MessageDto>>
        {
            Data = messages,
            Message = message,
            Status = StatusCodes.Status200OK
        });
    }

    /// <summary>
    /// Send group message
    /// </summary>
    [Authorize(Policy = "GroupMember")]
    [HttpPost("{groupGuid}/messages")]
    [RequestSizeLimit(20_000_000)]
    public async Task<ActionResult<GenericResponse<MessageDto>>> SendMessage(Guid groupGuid, [FromForm] SendMessageRequest request)
    {
        var (success, message, messages) = await _groupService.SendMessageAsync(groupGuid, request);

        if (!success)
            return BadRequest(ResponseHelper.Error<IEnumerable<MessageDto>>(message));

        await _hubContext.Clients.Group(groupGuid.ToString()).SendAsync("ReceiveMessage", messages, groupGuid);

        return Ok(new GenericResponse<MessageDto>
        {
            Data = messages,
            Message = message,
            Status = StatusCodes.Status200OK
        });
    }

    /// <summary>
    /// Get group roles
    /// </summary>
    [Authorize(Policy = "GroupMember")]
    [HttpGet("{groupGuid}/roles")]
    public async Task<ActionResult<GenericResponse<IEnumerable<GroupRoleDto>>>> GetGroupRoles(Guid groupGuid)
    {
        var (success, message, roles) = await _groupService.GetGroupRolesAsync(groupGuid);

        if (!success)
            return BadRequest(ResponseHelper.Error<IEnumerable<GroupRoleDto>>(message));

        return Ok(new GenericResponse<IEnumerable<GroupRoleDto>>
        {
            Data = roles,
            Message = message,
            Status = StatusCodes.Status200OK
        });
    }

    /// <summary>
    /// Add role to user in group
    /// </summary>
    [Authorize(Policy = "CanManageUserRoles")]
    [HttpPost("{groupGuid}/user/{userId}/roles")]
    public async Task<ActionResult<GenericResponse<AddRoleToUserDto?>>> AddRoleToUser(Guid groupGuid, string userId, [FromBody] AddRoleToUserRequest request)
    {
        var (success, message, dto) = await _groupService.AddRoleToUser(groupGuid, userId, request.RoleId);

        if (!success || dto == null)
            return BadRequest(ResponseHelper.Error<AddRoleToUserDto?>(message));

        await _hubContext.Clients.Group(groupGuid.ToString()).SendAsync("UserAddedRole", dto.GroupId, dto.UserId, dto.RoleId);

        return Ok(new GenericResponse<AddRoleToUserDto?>
        {
            Data = dto,
            Message = message,
            Status = StatusCodes.Status200OK
        });
    }

    /// <summary>
    /// Remove role from user in group
    /// </summary>
    [Authorize(Policy = "CanManageUserRoles")]
    [HttpDelete("{groupGuid}/user/{userId}/roles")]
    public async Task<ActionResult<GenericResponse<RemoveRoleFromUserDto?>>> RemoveRoleFromUser(Guid groupGuid, string userId, [FromBody] RemoveRoleFromUserRequest request)
    {
        var (success, message, dto) = await _groupService.RemoveRoleFromUser(groupGuid, userId, request.RoleId);

        if (!success || dto == null)
            return BadRequest(ResponseHelper.Error<RemoveRoleFromUserDto?>(message));

        await _hubContext.Clients.Group(groupGuid.ToString()).SendAsync("UserRemovedRole", dto.GroupId, dto.UserId, dto.RoleId);

        return Ok(new GenericResponse<RemoveRoleFromUserDto?>
        {
            Data = dto,
            Message = message,
            Status = StatusCodes.Status200OK
        });
    }

    /// <summary>
    /// Create group role
    /// </summary>
    [Authorize(Policy = "CanManageServerRoles")]
    [HttpPost("{groupGuid}/roles")]
    public async Task<ActionResult<GenericResponse<GroupRoleDto>>> CreateRole(Guid groupGuid, [FromBody] CreateGroupRoleRequest request)
    {
        var (success, message, dto) = await _groupService.CreateRoleAsync(groupGuid, request);

        if (!success)
            return BadRequest(ResponseHelper.Error<GroupRoleDto>(message));

        await _hubContext.Clients.Group(groupGuid.ToString()).SendAsync("RoleCreated", groupGuid, dto);

        return Ok(new GenericResponse<GroupRoleDto?>
        {
            Data = dto,
            Message = message,
            Status = StatusCodes.Status201Created
        });
    }


    /// <summary>
    /// Edit group role
    /// </summary>
    [Authorize(Policy = "CanManageServerRoles")]
    [HttpPut("{groupGuid}/roles/{roleId}")]
    public async Task<ActionResult<GenericResponse<GroupRoleDto>>> EditRole(Guid groupGuid, Guid roleId, [FromBody] EditGroupRoleRequest request)
    {
        var (success, message, dto) = await _groupService.EditRoleAsync(roleId, request);

        if (!success)
            return BadRequest(ResponseHelper.Error<GroupRoleDto>(message));

        await _hubContext.Clients.Group(groupGuid.ToString()).SendAsync("RoleEdited", groupGuid, dto);

        return Ok(new GenericResponse<GroupRoleDto>
        {
            Data = dto,
            Message = message,
            Status = StatusCodes.Status200OK
        });
    }

    [Authorize(Policy = "GroupMember")]
    [HttpPost("{groupGuid}/read")]
    public async Task<ActionResult<GenericResponse<object>>> MarkAsRead(Guid groupGuid, [FromBody] MarkAsReadRequest request)
    {
        var (success, message) = await _groupService.MarkAsReadAsync(groupGuid, request);
        if (!success)
            return BadRequest(ResponseHelper.Error<object>(message));

        return Ok(new GenericResponse<object>
        {
            Data = null,
            Message = message,
            Status = StatusCodes.Status200OK
        });
    }

    /// <summary>
    /// Ban member from group
    /// </summary>
    [Authorize(Policy = "CanBan")]
    [HttpPost("{groupGuid}/members/{userId}/ban")]
    public async Task<ActionResult<GenericResponse<object>>> BanUserFromGroup(Guid groupGuid, string userId, [FromBody] BanUserRequest request)
    {
        var (success, message) = await _groupService.BanUserFromGroupAsync(groupGuid, userId, request);

        if (!success)
            return BadRequest(ResponseHelper.Error<object>(message));

        await _hubContext.Clients.Group(groupGuid.ToString()).SendAsync("RemoveMember", groupGuid, userId);

        return Ok(new GenericResponse<object>
        {
            Message = message,
            Status = StatusCodes.Status200OK
        });
    }


    /// <summary>
    /// Get group bans
    /// </summary>
    [Authorize(Policy = "GroupMember")]
    [HttpGet("{groupGuid}/bans")]
    public async Task<ActionResult<GenericResponse<IEnumerable<BanDto>>>> GetGroupBans(Guid groupGuid)
    {
        var (success, message, dto) = await _groupService.GetGroupBansAsync(groupGuid);
        if (!success)
            return BadRequest(ResponseHelper.Error<IEnumerable<BanDto>>(message));

        return Ok(new GenericResponse<IEnumerable<BanDto>>
        {
            Message = message,
            Status = StatusCodes.Status200OK,
            Data = dto
        });
    }

    /// <summary>
    /// Delete specific ban in group
    /// </summary>
    [Authorize(Policy = "CanBan")]
    [HttpDelete("{groupGuid}/bans/{banGuid}")]
    public async Task<ActionResult<GenericResponse<object>>> DeleteGroupBan(Guid groupGuid, Guid banGuid)
    {
        var (success, message) = await _groupService.DeleteGroupBanAsync(groupGuid, banGuid);
        if (!success)
            return BadRequest(ResponseHelper.Error<object>(message));

        return Ok(new GenericResponse<object>
        {
            Message = message,
            Status = StatusCodes.Status200OK,
        });
    }
}
