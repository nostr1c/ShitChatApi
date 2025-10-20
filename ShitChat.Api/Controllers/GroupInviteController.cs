using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ShitChat.Api.Hubs;
using ShitChat.Application.DTOs;
using ShitChat.Application.Invites.DTOs;
using ShitChat.Application.Invites.Requests;
using ShitChat.Application.Invites.Services;

namespace ShitChat.Api.Controllers;

[Authorize(AuthenticationSchemes = "Bearer")]
[ApiController]
[Route("api/v1/group/{groupGuid}/invites")]
public class GroupInviteController : ControllerBase
{
    private readonly ILogger<GroupInviteController> _logger;
    private readonly IInviteService _inviteService;
    private readonly IHubContext<ChatHub> _hubContext;

    public GroupInviteController
    (
        ILogger<GroupInviteController> logger,
        IInviteService inviteService,
        IHubContext<ChatHub> hubContext
    )
    {
        _logger = logger;
        _inviteService = inviteService;
        _hubContext = hubContext;
    }

    [HttpPost]
    [Authorize(Policy = "CanManageInvites")]
    public async Task<ActionResult<GenericResponse<InviteDto?>>> CreateInvite(Guid groupGuid, [FromBody] CreateInviteRequest request)
    {
        var (success, message, inviteDto) = await _inviteService.CreateInviteAsync(groupGuid, request);

        if (!success)
            return BadRequest(ResponseHelper.Error<InviteDto?>(message));

        await _hubContext.Clients.Group(groupGuid.ToString()).SendAsync("ReceiveInvite", inviteDto, groupGuid);

        return Ok(ResponseHelper.Success(message, inviteDto, StatusCodes.Status201Created));
    }
    [HttpGet]
    [Authorize(Policy = "GroupMember")]
    public async Task<ActionResult<GenericResponse<IEnumerable<InviteDto>>>> GetGroupInvites(Guid groupGuid)
    {
        var (success, message, groupInviteDto) = await _inviteService.GetGroupInvites(groupGuid);

        if (!success)
            return BadRequest(ResponseHelper.Error<IEnumerable<InviteDto>>(message));

        return Ok(ResponseHelper.Success(message, groupInviteDto));
    }

    [Authorize(Policy = "CanManageInvites")]
    [HttpDelete("{inviteGuid}")]
    public async Task<ActionResult<GenericResponse<object?>>> DeleteInvite(Guid groupGuid, Guid inviteGuid)
    {
        var (success, message) = await _inviteService.DeleteInviteAsync(inviteGuid);

        if (!success)
            return BadRequest(ResponseHelper.Error<object?>(message));

        await _hubContext.Clients.Group(groupGuid.ToString()).SendAsync("DeletedInvite", groupGuid, inviteGuid);

        return Ok(ResponseHelper.Success(message));
    }
}
