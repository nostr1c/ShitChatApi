using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ShitChat.Api.Hubs;
using ShitChat.Application.DTOs;
using ShitChat.Application.Groups.Services;
using ShitChat.Application.Invites.DTOs;

namespace ShitChat.Api.Controllers;

[Authorize(AuthenticationSchemes = "Bearer")]
[ApiController]
[Route("api/v1/invite")]
public class InviteController : ControllerBase
{
    private readonly IGroupService _groupService;
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly ILogger<InviteController> _logger;

    public InviteController(
        IGroupService groupService,
        IHubContext<ChatHub> hubContext,
        ILogger<InviteController> logger)
    {
        _groupService = groupService;
        _hubContext = hubContext;
        _logger = logger;
    }

    /// <summary>
    /// Join a group using an invite code
    /// </summary>
    [HttpPost("{inviteCode}/join")]
    public async Task<ActionResult<GenericResponse<JoinInviteDto?>>> JoinWithInvite(string inviteCode)
    {
        var (success, message, joinInviteDto) = await _groupService.JoinWithInviteAsync(inviteCode);

        if (!success || joinInviteDto is null)
            return BadRequest(ResponseHelper.Error<JoinInviteDto>(message));

        var groupGuid = joinInviteDto.Group.Id;

        await _hubContext.Clients.Group(groupGuid.ToString())
            .SendAsync("ReceiveMember", groupGuid, joinInviteDto.Member);

        return Ok(new GenericResponse<JoinInviteDto>
        {
            Data = joinInviteDto,
            Message = message
        });
    }
}
