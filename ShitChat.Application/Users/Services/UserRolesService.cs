using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShitChat.Application.Roles.DTOs;
using ShitChat.Application.Users.DTOs;
using ShitChat.Domain.Entities;
using ShitChat.Infrastructure.Data;

namespace ShitChat.Application.Users.Services;

public class UserRolesService : IUserRolesService
{
    private readonly RoleManager<AppRole> _roleManager;
    private readonly UserManager<User> _userManager;
    private readonly AppDbContext _dbContext;

    public UserRolesService
    (
        RoleManager<AppRole> roleManager,
        UserManager<User> userManager,
        AppDbContext dbContext
    )
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public async Task<(bool, string, IEnumerable<RoleDto>?)> GetUserRoles(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return (false, "ErrorUserNotFound", null);

        var roleNames = await _userManager.GetRolesAsync(user);

        var rolesDto = await _roleManager.Roles
            .Where(x => roleNames.Contains(x.Name))
            .Select(x => new RoleDto
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToListAsync();

        return (true, "SuccessGotUserRoles", rolesDto);
    }

    public async Task<(bool, string, UserWithRoles?)> AddRoleToUser(string userId, string roleId)
    {
        var roleName = await _roleManager.Roles
            .Where(x => x.Id == roleId)
            .Select(x => x.Name)
            .SingleOrDefaultAsync();

        if (roleName == null)
            return (false, "ErrorRoleNotFound", null);

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null) 
            return (false, "ErrorUserNotFound", null);

        await _userManager.AddToRoleAsync(user, roleName);

        var userDto = new UserWithRoles
        {
            User = new UserDto
            {
                Id = user.Id,
                Username = user.UserName,
                Avatar = user.AvatarUri,
                CreatedAt = user.CreatedAt,
                Email = user.Email,
            },
            Roles = user.UserRoles.Select(x => new RoleDto
            {
                Id = x.RoleId,
                Name = x.Role.Name
            }).ToList()
        }; 

        return (true, "SuccessAddingRoleToUser", userDto);
    }

    public async Task<(bool, string)> RemoveRoleFromUser(string userId, string roleId)
    {
        var roleName = await _roleManager.Roles
        .Where(x => x.Id == roleId)
        .Select(x => x.Name)
        .SingleOrDefaultAsync();

        if (roleName == null)
            return (false, "ErrorRoleNotFound");

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return (false, "ErrorUserNotFound");

        await _userManager.RemoveFromRoleAsync(user, roleName);

        return (true, "SuccessRemovingRoleToUser");
    }
}
