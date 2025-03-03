using ShitChat.Infrastructure.Data;
using ShitChat.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace ShitChat.Api.Authorization;

public class GroupMembershipHandler : AuthorizationHandler<GroupMembershipRequirement>
{
    private readonly AppDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GroupMembershipHandler(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, GroupMembershipRequirement requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var userId = httpContext.User.GetUserGuid();
        if (userId == null)
        {
            context.Fail();
            return;
        }

        if (!httpContext.Request.RouteValues.TryGetValue(requirement.GroupIdParameter, out var groupId) || !Guid.TryParse(groupId?.ToString(), out var groupGuid))
        {
            context.Fail();
            return;
        }

        var isMember = await _dbContext.Groups
            .AnyAsync(g => g.Id == groupGuid && g.Users.Any(u => u.Id == userId));

        if (isMember)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }
}
