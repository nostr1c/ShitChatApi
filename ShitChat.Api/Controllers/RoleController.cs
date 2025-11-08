using Microsoft.AspNetCore.Mvc;
using ShitChat.Application.DTOs;
using ShitChat.Application.Roles.DTOs;
using ShitChat.Application.Roles.Services;

namespace ShitChat.Api.Controllers;

//[Authorize(AuthenticationSchemes = "Bearer")]
[ApiController]
[Route("api/v1/[controller]")]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    public async Task<ActionResult<GenericResponse<IEnumerable<RoleDto>>>> GetRoles()
    {
        var (success, message, roles) = await _roleService.GetRolesAsync();

        if (!success)
            return BadRequest(ResponseHelper.Error<IEnumerable<RoleDto>>(message));

        return Ok(ResponseHelper.Success(message, roles));
    }

    [HttpPost]
    public async Task<ActionResult<GenericResponse<RoleDto>>> CreateRole(string name)
    {
        var (success, message, role) = await _roleService.CreateRoleAsync(name);

        if (!success)
            return BadRequest(ResponseHelper.Error<RoleDto>(message));

        return Ok(ResponseHelper.Success(message, role));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<GenericResponse<RoleDto>>> DeleteRole(string id)
    {
        var (success, message) = await _roleService.DeleteRoleAsync(id);

        if (!success)
            return BadRequest(ResponseHelper.Error<RoleDto>(message));

        return Ok(ResponseHelper.Success(message));
    }
}
