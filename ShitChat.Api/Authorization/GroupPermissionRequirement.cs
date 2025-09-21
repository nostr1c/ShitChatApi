using Microsoft.AspNetCore.Authorization;

namespace ShitChat.Api.Authorization;

public class GroupPermissionRequirement : IAuthorizationRequirement
{
    public string GroupIdParameter { get; }
    public string PermissionName { get; }

    public GroupPermissionRequirement(string permissionName, string groupIdParameter = "groupGuid")
    {
        GroupIdParameter = groupIdParameter;
        PermissionName = permissionName;
    }
}
