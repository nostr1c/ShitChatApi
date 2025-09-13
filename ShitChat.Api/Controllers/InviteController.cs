using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ShitChat.Api.Hubs;
using ShitChat.Application.DTOs;
using ShitChat.Application.Interfaces;
using ShitChat.Application.Requests;

namespace ShitChat.Api.Controllers;

[Authorize(AuthenticationSchemes = "Bearer")]
[ApiController]
[Route("api/v1/[controller]")]
public class InviteController : ControllerBase
{
    private readonly ILogger<InviteController> _logger;
    private readonly IInviteService _inviteService;
    private readonly IHubContext<ChatHub> _hubContext;

    public InviteController
    (
        ILogger<InviteController> logger,
        IInviteService inviteService,
        IHubContext<ChatHub> hubContext
    )
    {
        _logger = logger;
        _inviteService = inviteService;
        _hubContext = hubContext;
    }

    [Authorize(Policy = "GroupMember")]
    [HttpPost("{groupGuid}")]
    public async Task<ActionResult<GenericResponse<InviteDto?>>> CreateInvite(Guid groupGuid, [FromBody] CreateInviteRequest request)
    {
        var (success, message, inviteDto) = await _inviteService.CreateInviteAsync(groupGuid, request);

        if (!success)
            return BadRequest(ResponseHelper.Error<InviteDto?>(message));

        await _hubContext.Clients.Group(groupGuid.ToString()).SendAsync("ReceiveInvite", inviteDto, groupGuid);

        return Ok(new GenericResponse<InviteDto?>
        {
            Data = inviteDto,
            Message = "SuccessCreatedInvite",
            Status = StatusCodes.Status201Created
        });
    }

    [HttpPost("join/{inviteString}")]
    public async Task<ActionResult<GenericResponse<JoinInviteDto?>>> JoinWithInvite(string inviteString)
    {
        var (success, message, joinInviteDto) = await _inviteService.JoinWithInviteAsync(inviteString);

        if (!success || joinInviteDto is null)
            return BadRequest(ResponseHelper.Error<JoinInviteDto>(message));

        var groupGuid = joinInviteDto.Group;

        await _hubContext.Clients.Group(groupGuid.ToString()).SendAsync("ReceiveMember", groupGuid, joinInviteDto.Member);

        return Ok(new GenericResponse<JoinInviteDto>
        {
            Data = joinInviteDto,
            Message = message
        });
    }

    [Authorize(Policy = "GroupMember")]
    [HttpGet("{groupGuid}")]
    public async Task<ActionResult<GenericResponse<IEnumerable<InviteDto>>>> GetGroupInvites(Guid groupGuid)
    {
        var (success, message, groupInviteDto) = await _inviteService.GetGroupInvites(groupGuid);

        if (!success)
            return BadRequest(ResponseHelper.Error<IEnumerable<InviteDto>>(message));

        return Ok(new GenericResponse<IEnumerable<InviteDto>>
        {
            Data = groupInviteDto,
            Message = message
        });
    }
}
