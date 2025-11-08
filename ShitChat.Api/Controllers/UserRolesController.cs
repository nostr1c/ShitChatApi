using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShitChat.Application.DTOs;
using ShitChat.Application.Roles.DTOs;
using ShitChat.Application.Users.Services;

namespace ShitChat.Api.Controllers;

//[Authorize(AuthenticationSchemes = "Bearer")]
[ApiController]
[Route("api/v1/user/{userId}/roles")]
public class UserRolesController : ControllerBase
{
    private readonly IUserRolesService _userRolesService;

    public UserRolesController(IUserRolesService userRolesService)
    {
        _userRolesService = userRolesService;
    }

    [HttpGet]
    public async Task<ActionResult<GenericResponse<IEnumerable<RoleDto>>>> GetUserRoles(string userId)
    {
        var (success, message, roles) = await _userRolesService.GetUserRoles(userId);

        if (!success) 
            return BadRequest(ResponseHelper.Error<IEnumerable<RoleDto>>(message));

        return Ok(ResponseHelper.Success(message, roles));
    }


    [HttpPost("{roleId}")]
    public async Task<ActionResult<GenericResponse<object>>> AddRoleToUser(string userId, string roleId)
    {
        var (success, message, user) = await _userRolesService.AddRoleToUser(userId, roleId);
        if (!success)
            return BadRequest(ResponseHelper.Error<object>(message));

        return Ok(ResponseHelper.Success(message, user));
    }

    [HttpDelete("{roleId}")]
    public async Task<ActionResult<GenericResponse<object>>> RemoveRoleFromuser(string userId, string roleId)
    {
        var (success, message) = await _userRolesService.RemoveRoleFromUser(userId, roleId);
        if (!success)
            return BadRequest(ResponseHelper.Error<object>(message));

        return Ok(ResponseHelper.Success(message));
    }
}
