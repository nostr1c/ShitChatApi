using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShitChat.Application.Roles.DTOs;
using ShitChat.Domain.Entities;

namespace ShitChat.Application.Roles.Services;

public class RoleService : IRoleService
{
    private readonly RoleManager<AppRole> _roleManager;

    public RoleService(RoleManager<AppRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<(bool, string, IEnumerable<RoleDto>)> GetRolesAsync()
    {
        var roles = await _roleManager.Roles
            .AsNoTracking()
            .Select(x => new RoleDto
            {
                Id = x.Id,
                Name = x.Name
            }).ToListAsync();

        return (true, "SuccessGotRoles", roles);
    }

    public async Task<(bool, string, RoleDto?)> CreateRoleAsync(string name)
    {
        var role = new AppRole
        {
            Name = name,
        };

        var result = await _roleManager.CreateAsync(role);
        if (!result.Succeeded)
            return (false, "ErrorCreatingRole", null);

        var roleDto = new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
        };

        return (true, "SuccessCreatingRole", roleDto);
    }

    public async Task<(bool, string)> DeleteRoleAsync(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);

        if (role == null)
            return (false, "ErrorRoleNotFound");

        await _roleManager.DeleteAsync(role);

        return (true, "SuccessDeletingRole");
    }
}
