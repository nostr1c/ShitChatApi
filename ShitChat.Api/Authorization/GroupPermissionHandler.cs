using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ShitChat.Infrastructure.Data;
using ShitChat.Shared.Extensions;

namespace ShitChat.Api.Authorization;

public class GroupPermissionHandler : AuthorizationHandler<GroupPermissionRequirement>
{
    private readonly AppDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GroupPermissionHandler(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, GroupPermissionRequirement requirement)
    {
        string userId;
        try {
            userId = _httpContextAccessor.GetUserId();
        }
        catch
        {
            context.Fail();
            return;
        }

        if (!_httpContextAccessor.HttpContext.Request.RouteValues.TryGetValue(requirement.GroupIdParameter, out var groupId) || !Guid.TryParse(groupId?.ToString(), out var groupGuid))
        {
            context.Fail();
            return;
        }

        var isOwner = await _dbContext.Groups
            .AnyAsync(g => g.Id == groupGuid && g.OwnerId == userId);

        if (isOwner)
        {
            context.Succeed(requirement);
            return;
        }

        var hasPermission = await _dbContext.UserGroupRoles
            .Where(ugr => ugr.UserId == userId && ugr.GroupRole.GroupId == groupGuid)
            .SelectMany(ugr => ugr.GroupRole.Permissions)
            .AnyAsync(grp => grp.Permission.Name == requirement.PermissionName);

        if (!hasPermission)
        {
            context.Fail();
        }
        else
        {
            context.Succeed(requirement);
        }
    }
}
