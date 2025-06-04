using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShitChat.Application.DTOs;
using ShitChat.Application.Interfaces;
using ShitChat.Application.Requests;
using ShitChat.Domain.Entities;
using ShitChat.Infrastructure.Data;
using ShitChat.Shared.Extensions;

namespace ShitChat.Application.Services;

public class RoleService : IRoleService
{
    private readonly AppDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RoleService
    (
        AppDbContext dbContext,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<(bool, string, GroupRoleDto?)> CreateRoleAsync(Guid groupId, CreateGroupRoleRequest request)
    {
        var userId = _httpContextAccessor.HttpContext.User.GetUserGuid();
        var user = await _dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            return (false, "ErrorLoggedInUser", null);

        var group = await _dbContext.Groups
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == groupId);

        if (group == null)
            return (false, "ErrorGroupNotFound", null);

        var groupRole = new GroupRole
        {
            GroupId = groupId,
            Color = request.Color ?? "gray",
            Name = request.Name,
        };

        _dbContext.GroupRoles.Add(groupRole);
        await _dbContext.SaveChangesAsync();

        var groupRoleDto = new GroupRoleDto
        {
            Id = groupRole.Id,
            Name = groupRole.Name,
            Color = groupRole.Color
        };

        return (true, "SuccessCreatedGroupRole", groupRoleDto);
    }
}
