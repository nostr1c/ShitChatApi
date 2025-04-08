using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShitChat.Application.DTOs;
using ShitChat.Application.Interfaces;
using ShitChat.Application.Requests;

namespace ShitChat.Api.Controllers;

[Authorize(AuthenticationSchemes = "Bearer")]
[ApiController]
[Route("api/v1/[controller]")]
public class InviteController : ControllerBase
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<GroupController> _logger;
    private readonly IInviteService _inviteService;

    public InviteController
    (
        IServiceProvider serviceProvider,
        ILogger<GroupController> logger,
        IInviteService inviteService
    )
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _inviteService = inviteService;
    }

    [Authorize(Policy = "GroupMember")]
    [HttpPost("{groupGuid}")]
    public async Task<ActionResult<GenericResponse<InviteDto?>>> CreateInvite(Guid groupGuid, [FromBody] CreateInviteRequest request)
    {
        var validator = _serviceProvider.GetRequiredService<IValidator<CreateInviteRequest>>();
        var validationResult = await validator.ValidateAsync(request);
        var response = new GenericResponse<InviteDto?>();

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

        var (success, message, inviteDto) = await _inviteService.CreateInviteAsync(groupGuid, request);

        if (!success)
        {
            response.Message = message;
            response.Data = null;
            return BadRequest(response);
        }

        response.Data = inviteDto;
        response.Message = "SuccessCreatedInvite";

        return Ok(response);
    }

    [HttpPost("join/{inviteString}")]
    public async Task<ActionResult<GenericResponse<JoinInviteDto?>>> JoinWithInvite(string inviteString)
    {
        var response = new GenericResponse<JoinInviteDto?>();
        var (success, message, joinInviteDto) = await _inviteService.JoinWithInviteAsync(inviteString);

        if (!success)
        {
            response.Message = message;
            response.Data = null;
        }

        response.Message = message;
        response.Data = joinInviteDto;

        return Ok(response);
    }
}
