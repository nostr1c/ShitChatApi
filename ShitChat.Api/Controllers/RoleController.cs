using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ShitChat.Application.DTOs;
using ShitChat.Application.Interfaces;
using ShitChat.Application.Requests;
using ShitChat.Application.Services;

namespace ShitChat.Api.Controllers;

[Authorize(AuthenticationSchemes = "Bearer")]
[ApiController]
[Route("api/v1/[controller]")]
public class RoleController : Controller
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IRoleService _roleService;
    private readonly ILogger<RoleController> _logger;

    public RoleController
    (
        IServiceProvider serviceProvider,
        IRoleService roleService,
        ILogger<RoleController> logger
    )
    {
        _serviceProvider = serviceProvider;
        _roleService = roleService;
        _logger = logger;
    }

    [HttpPost("{groupGuid}")]
    public async Task<ActionResult<GenericResponse<GroupRoleDto>>> CreateRole(Guid groupGuid, CreateGroupRoleRequest request)
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

        var (success, message, inviteDto) = await _roleService.CreateRoleAsync(groupGuid, request);

        if (!success)
        {
            response.Message = message;
            response.Data = null;
            return BadRequest(response);
        }
        response.Data = inviteDto;
        response.Message = "SuccessCreatedGroupRole";

        return Ok(response);
    }
}
